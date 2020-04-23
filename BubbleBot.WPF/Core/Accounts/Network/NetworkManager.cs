using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Enums;
using BubbleBot.Core.Frames;
using BubbleBot.Core.Network;
using BubbleBot.Protocol.Messages;
using BubbleBot.Utility;
using BubbleBot.Utility.DofusTouch;
using BubbleBot.Utility.Extensions;
using BubbleBot.Utility.Security;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BubbleBot.Core.Accounts.Network
{
    public class NetworkManager : ViewModelBase, IClearable, IDisposable
    {
        // Fields
        private static readonly List<string> MessagesToIgnore = new List<string>
        {
            "ChatServerMessage", "BasicLatencyStatsRequestMessage", "BasicLatencyStatsMessage",
            "SequenceNumberRequestMessage", "SequenceNumberMessage", "BasicPingMessage"
        };

        private string _access;
        private NetworkPhases _phase;
        private Timer _pingTimer;
        private string _primus;
        private ConcurrentDictionary<string, RegisteredMessage> _registeredMessages;
        private SemaphoreSlim _semaphore;
        private string _serverAddress;
        private int _serverId;
        private uint _serverPort;
        private string _sessionId;
        private string _sid;
        private PrimusWebSocket _webSocket;

        public Timer ConnectTimeout;


        // Constructor
        public NetworkManager(Account account)
        {
            Account = account;
            _phase = NetworkPhases.NONE;
            _webSocket = new PrimusWebSocket();
            _pingTimer = new Timer(PingTimerCallback, null, 600000, 600000);
            _semaphore = new SemaphoreSlim(1);
            _registeredMessages = new ConcurrentDictionary<string, RegisteredMessage>();
            Messages = new List<NetworkMessage>();

            AddEvents();
        }

        // Properties
        public Account Account { get; private set; }
        public bool Connected { get; private set; }

        public NetworkPhases Phase
        {
            get => _phase;
            internal set
            {
                var oldValue = _phase;
                _phase = value;

                if (oldValue != _phase)
                    PhaseChanged?.Invoke(this);
            }
        }

        public List<NetworkMessage> Messages { get; private set; }

        public void Clear()
        {
            _sessionId = null;
            _serverId = 0;
            _serverAddress = null;
            _serverPort = 0;
            _primus = null;
            _access = null;
            _sid = null;
            if (ConnectTimeout != null)
                ConnectTimeout.Change(Timeout.Infinite, Timeout.Infinite);
            ConnectTimeout = null;
        }


        // Events
        public event Action<NetworkManager> PhaseChanged;
        public event Action<NetworkManager> Disconnected;


        public async Task ConnectToLoginServer()
        {
            if (Connected)
                return;

            if (Phase != NetworkPhases.NONE)
                Phase = NetworkPhases.NONE;

            _sessionId = 16.ToRandomString();
            _primus = YeastAPI.GenerateKey();

            ConnectTimeout = new Timer(ConnectTimeoutCallback, null, 120000, 120000);

            // Url as null if it is the first time then we use the selected server as url
            if (!await SetSid(null, _sessionId, Account.AccountConfig.Proxy.Ip ?? "",
                Account.AccountConfig.Proxy.Port.ToString(), Account.AccountConfig.Proxy.Username ?? "",
                Account.AccountConfig.Proxy.Password ?? ""))
            {
                if (Account.AccountConfig.Proxy.IsValid)
                    Account.Logger.LogError("", LanguageManager.Translate("672"));
                else
                    Account.Logger.LogError("", LanguageManager.Translate("673"));

                Account.Network?.Disconnected?.Invoke(this);
                Account.IsIntentionalDisconnection = !GlobalConfiguration.Instance.AutomaticReconnection;

                Account.Network.Clear();
                return;
            }

            await _webSocket.OpenAsync(
                $"wss://proxyconnection.touch.dofus.com/primus/?STICKER={_sessionId}&_primuscb={_primus}&EIO=3&transport=websocket",
                _sid,
                Account.AccountConfig.Proxy.Url ?? null, Account.AccountConfig.Proxy.Username ?? null,
                Account.AccountConfig.Proxy.Password ?? null);
        }


        public async Task SwitchToGameServer(string address, uint port, int serverId, string access)
        {
            if (!Connected || Phase != NetworkPhases.LOGIN)
            {
                _phase = NetworkPhases.NONE;
                Connected = false;
                await _webSocket.CloseAsync("CLIENT_CLOSING").ConfigureAwait(false);
                Account.IsIntentionalDisconnection = false;
                Account.Network.Clear();
                Account.Network.Disconnected?.Invoke(this);
                return;
            }

            _serverAddress = address;
            _serverPort = port;
            _serverId = serverId;
            _access =
                $"{access.Replace("https", "wss")}/primus/?STICKER={_sessionId}&_primuscb={_primus}&EIO=3&transport=websocket";
            Phase = NetworkPhases.SWITCHING_TO_GAME;

            await Disconnect("SWITCHING_TO_GAME").ConfigureAwait(false);
        }

        public async Task Disconnect(string reason, bool unintentional = false)
        {
            try
            {
                Account.FightLimitReached = false;

                if (!Connected)
                    return;

                // If it is set to true, it simulates an unintentional disconnection
                if (unintentional == false) Account.IsIntentionalDisconnection = true;

                await _webSocket.CloseAsync(reason).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Disconnection exception: {0}", ex.Message);
            }
        }

        public string RegisterMessage<T>(Func<Account, T, Task> action) where T : Message
        {
            if (_disposedValue)
                return null;

            Task NewAction(Account a, object o)
            {
                return action(a, (T) o);
            }

            var id = 16.ToRandomString();
            _registeredMessages.TryAdd(id, new RegisteredMessage(typeof(T), NewAction));

            return id;
        }

        public bool UnregisterMessage(string id)
        {
            return _registeredMessages.TryRemove(id, out var rm);
        }

        // Useful to get maps or item data
        public async Task<string> PerformCurlAsync(string urlString, string content)
        {
            var handler = new HttpClientHandler();
            handler.UseCookies = false;

            handler.AutomaticDecompression = ~DecompressionMethods.None;

            if (Account.AccountConfig.Proxy.IsValid)
                if (Account.AccountConfig.Proxy.Ip != "" && Account.AccountConfig.Proxy.Port != 0)
                {
                    var proxy = new WebProxy
                    {
                        Address =
                            new Uri($"http://{Account.AccountConfig.Proxy.Ip}:{Account.AccountConfig.Proxy.Port}"),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false
                    };

                    if (Account.AccountConfig.Proxy.Username != "" && Account.AccountConfig.Proxy.Password != "")
                        proxy.Credentials = new NetworkCredential
                        {
                            UserName = Account.AccountConfig.Proxy.Username,
                            Password = Account.AccountConfig.Proxy.Password
                        };

                    handler.Proxy = proxy;
                }

            using (var httpClient = new HttpClient(handler, true))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), urlString))
                {
                    request.Headers.TryAddWithoutValidation("Origin", "file://");
                    request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                    request.Headers.TryAddWithoutValidation("Accept-Language", "fr");
                    request.Headers.TryAddWithoutValidation("User-Agent",
                        "Mozilla/5.0 (Linux; Android 7.0; Nexus 5X Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.118 Mobile Safari/537.36");
                    request.Headers.TryAddWithoutValidation("Accept", "*/*");
                    request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                    request.Headers.TryAddWithoutValidation("Cookie", "io=" + _sid);

                    request.Content = new StringContent(content);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                        return response.Content.ReadAsStringAsync().Result;
                }
            }

            return null;
        }

        public string PerformXmlHttpRequest(string urlString, string xmlContent)
        {
            string response = null;
            HttpWebResponse httpWebResponse = null; //Declare an HTTP-specific implementation of the WebResponse class
            //Creates an HttpWebRequest for the specified URL.
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(urlString);

            try
            {
                byte[] bytes;
                bytes = Encoding.ASCII.GetBytes(xmlContent);
                //Set HttpWebRequest properties
                httpWebRequest.Headers.Add("Origin", "file://");
                httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                httpWebRequest.Headers.Add("Accept-Language", "fr");
                httpWebRequest.UserAgent =
                    "Mozilla/5.0 (Linux; Android 7.0; Nexus 5X Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.118 Mobile Safari/537.36";
                httpWebRequest.ContentType = "application/json'";
                httpWebRequest.Accept = "*/*";
                httpWebRequest.Headers.Add("Cookie", "io=" + _sid);
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.Method = "POST";

                if (Account.AccountConfig.Proxy.IsValid)
                    if (Account.AccountConfig.Proxy.Ip != "" && Account.AccountConfig.Proxy.Port != 0)
                    {
                        var proxy = new WebProxy
                        {
                            Address = new Uri(
                                $"http://{Account.AccountConfig.Proxy.Ip}:{Account.AccountConfig.Proxy.Port}"),
                            BypassProxyOnLocal = false,
                            UseDefaultCredentials = false
                        };

                        if (Account.AccountConfig.Proxy.Username != "" && Account.AccountConfig.Proxy.Password != "")
                            proxy.Credentials = new NetworkCredential
                            {
                                UserName = Account.AccountConfig.Proxy.Username,
                                Password = Account.AccountConfig.Proxy.Password
                            };

                        httpWebRequest.Proxy = proxy;
                    }

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    //Writes a sequence of bytes to the current stream 
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close(); //Close stream
                }

                //Sends the HttpWebRequest, and waits for a response.
                httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    //Get response stream into StreamReader
                    using (var responseStream = httpWebResponse.GetResponseStream())
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            response = reader.ReadToEnd();
                        }
                    }

                httpWebResponse.Close(); //Close HttpWebResponse
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            httpWebResponse = null;
            httpWebRequest = null;
            return response;
        }

        private async void PingTimerCallback(object state)
        {
            if (Connected)
                await Account.Network.SendMessageAsync(new BasicPingMessage(true));
            else
                _pingTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private async void ConnectTimeoutCallback(object state)
        {
            if (Account.Game.Character.IsSelected && Account.State == AccountStates.CONNECTING &&
                !Account.PreventAutoReconnection)
            {
                _phase = NetworkPhases.NONE;
                try
                {
                    await _webSocket.CloseAsync("CLIENT_CLOSING").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occured in PWS while closing (already closed?): {0}", ex.Message);
                    Account.Network?.Disconnected?.Invoke(this);
                }

                Account.IsIntentionalDisconnection = !GlobalConfiguration.Instance.AutomaticReconnection;
                Account.Network?.Clear();
            }

            ConnectTimeout.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void AddEvents()
        {
            _webSocket.Opened += WebSocket_Opened;
            _webSocket.Closed += WebSocket_Closed;
            _webSocket.MessageReceived += WebSocket_MessageReceived;
            _webSocket.ErrorOccured += WebSocket_ErrorOccured;
        }

        private void RemoveEvents()
        {
            _webSocket.Opened -= WebSocket_Opened;
            _webSocket.Closed -= WebSocket_Closed;
            _webSocket.MessageReceived -= WebSocket_MessageReceived;
            _webSocket.ErrorOccured -= WebSocket_ErrorOccured;
        }

        private void AddMessage(string message, bool sent)
        {
            if (Messages.Count >= 200)
                Messages.RemoveAt(0);

            Messages.Add(new NetworkMessage(message, sent));
        }

        #region Send Messages

        public async Task SendRawAsync(string text)
        {
            if (!Connected || _semaphore == null)
                return;

            await _semaphore.WaitAsync();

            await _webSocket.SendAsync(text).ConfigureAwait(false);

            _semaphore.Release();
        }

        public async Task SendCallAsync(Message message)
        {
            if (!Connected)
                return;

            await _semaphore.WaitAsync();

            var json = message.ToCall();
            await _webSocket.SendAsync(json).ConfigureAwait(false);

            // Register all messages except these ones
            if (!MessagesToIgnore.Contains(message.GetType().Name)) AddMessage(json, true);

            _semaphore.Release();
        }

        public async Task SendMessageAsync(Message message)
        {
            if (!Connected)
                return;

            await _semaphore.WaitAsync();

            var json = message.ToSendMessage();
            await _webSocket.SendAsync(json).ConfigureAwait(false);

            // Register all messages except these ones
            if (!MessagesToIgnore.Contains(message.GetType().Name)) AddMessage(json, true);

            _semaphore.Release();
        }

        public void SendMessage(Message message)
        {
            SendMessageAsync(message).Wait();
        }

        #endregion

        #region Received Events

        private void WebSocket_ErrorOccured(PrimusWebSocket ws, Exception ex)
        {
            if (ex == null)
                return;
            Console.WriteLine("websocket-error: {0}", ex);
            Account.Logger.LogError("", ex.ToString());
        }

        private void WebSocket_MessageReceived(PrimusWebSocket ws, JObject json)
        {
            var messageType = json["_messageType"].ToString();
            //Console.WriteLine("messagetype recu: " + messageType); //123456
            try
            {
                var message = MessagesBuilder.GetMessage(messageType, json);
                if (message == null)
                {
                    Console.WriteLine($"Message not found: {messageType}");
                    return;
                }
                //Console.WriteLine("message recu: " + message); //123456

                // Register all messages except these ones
                if (!MessagesToIgnore.Contains(messageType)) AddMessage(json.ToString(Formatting.None), false);


                FramesManager.HandleMessage(Account, message);

                foreach (var rm in _registeredMessages.Values)
                {
                    if (message.GetType() != rm.Type)
                        continue;

                    // In case the account was disposed
                    if (_disposedValue)
                        return;

                    //rm.Action.Invoke(Account, message).ContinueWith(c => c.Exception.InnerException.SendCrashReport(),
                    //TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
                    rm.Action.Invoke(Account, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : {0}", ex.Message);
            }
        }

        private async void WebSocket_Closed(PrimusWebSocket ws)
        {
            Connected = false;
            Disconnected?.Invoke(this);

            try
            {
                if (Phase == NetworkPhases.SWITCHING_TO_GAME && _access != null && Account != null && _sid != null)
                {
                    // We have to retrieve the sid from the server
                    if (!await SetSid(_access, _sessionId, Account.AccountConfig.Proxy.Ip ?? "",
                        Account.AccountConfig.Proxy.Port.ToString(), Account.AccountConfig.Proxy.Username ?? "",
                        Account.AccountConfig.Proxy.Password ?? ""))
                    {
                        if (Account.AccountConfig.Proxy.IsValid)
                            Account.Logger.LogError("", LanguageManager.Translate("672"));
                        else
                            Account.Logger.LogError("", LanguageManager.Translate("673"));

                        Phase = NetworkPhases.NONE;
                        Account.Network?.Disconnected?.Invoke(this);
                        Account.IsIntentionalDisconnection = !GlobalConfiguration.Instance.AutomaticReconnection;
                        return;
                    }

                    if (_access == null || _access == "")
                        return;

                    // Connecting to the game server
                    if (Account.AccountConfig.Proxy.IsValid)
                        await _webSocket.OpenAsync(_access, _sid, Account.AccountConfig.Proxy.Url,
                            Account.AccountConfig.Proxy.Username, Account.AccountConfig.Proxy.Password);
                    else
                        await _webSocket.OpenAsync(_access, _sid);

                    Account.State = AccountStates.CONNECTING;
                }
                else
                {
                    Phase = NetworkPhases.NONE;
                }
            }
            catch
            {
            }
        }

        private async void WebSocket_Opened(PrimusWebSocket ws)
        {
            Connected = true;

            // Connecting to the login server
            if (Phase == NetworkPhases.NONE)
            {
                var cm = new ConnectingMessage(DTConstants.AppVersion, DTConstants.BuildVersion,
                    GlobalConfiguration.Instance.Lang, "login", "android");
                await SendCallAsync(cm).ConfigureAwait(false);
            }
            else if (Phase == NetworkPhases.SWITCHING_TO_GAME)
            {
                // Since there is already a ConnectMessage class, i had to write this manually
                dynamic msg = new ExpandoObject();
                msg.call = "connecting";
                msg.data = new ExpandoObject();
                msg.data.appVersion = DTConstants.AppVersion;
                msg.data.buildVersion = DTConstants.BuildVersion;
                msg.data.client = "android";
                msg.data.language = GlobalConfiguration.Instance.Lang;
                msg.data.server = new ExpandoObject();
                msg.data.server.address = _serverAddress;
                msg.data.server.port = _serverPort;
                msg.data.server.id = _serverId;

                string raw = JsonConvert.SerializeObject(msg);
                await SendRawAsync(raw);
            }
        }

        private async Task<bool> SetSid(string url = null, string sticker = null, string host = "",
            string service = "0", string username = "", string password = "")
        {
            var yeastValue = YeastAPI.GenerateKey();
            string fullUrl;
            if (url == null)
            {
                fullUrl = "https://proxyconnection.touch.dofus.com/primus/?STICKER=" + sticker + "&_primuscb=" +
                          _primus + "&EIO=3&transport=polling&t=" + yeastValue + "&b64=1";
            }
            else
            {
                var tempUrl = url.Substring(0, url.LastIndexOf('&')) + "&_primuscb=" + _primus +
                              "&EIO=3&transport=polling&t=" + yeastValue + "&b64=1";
                fullUrl = tempUrl.Replace("wss", "https");
            }

            HttpClient client;
            if (host != "" && service != "0")
            {
                // First create a proxy object
                var proxy = new WebProxy
                {
                    Address = new Uri($"http://{host}:{service}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false
                };

                if (service != "" && username != "")
                    proxy.Credentials = new NetworkCredential
                    {
                        UserName = username,
                        Password = password
                    };
                // Now create a client handler which uses that proxy
                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy
                };

                client = new HttpClient(httpClientHandler, true);
            }
            else
            {
                client = new HttpClient();
            }

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("UserAgent",
                "Mozilla/5.0 (Linux; Android 7.1.1; ONEPLUS A" + Randomize.GetRandomInt(1, 10000) +
                "Build/NMF26F; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/59.0.3071.92 Mobile Safari/537.36");

            try
            {
                var response = await client.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    string result = null;
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }

                    var dictionaryRes =
                        JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(result)
                            .Substring(result.IndexOf('{')));
                    _sid = (string) dictionaryRes["sid"];
                    if (_webSocket != null)
                    {
                        _webSocket.SocketPingInterval = (long) dictionaryRes["pingInterval"];
                        _webSocket.SocketPingTimeout = (long) dictionaryRes["pingTimeout"];
                    }

                    response.Dispose();
                    client.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : {0}", ex.Message);
                client.Dispose();
                return false;
            }

            client.Dispose();
            return false;
        }

        #endregion

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (ConnectTimeout != null)
                    ConnectTimeout?.Change(Timeout.Infinite, Timeout.Infinite);

                if (disposing)
                {
                    ConnectTimeout?.Dispose();
                    _pingTimer.Dispose();
                    _semaphore.Dispose();
                }

                _phase = NetworkPhases.NONE;
                RemoveEvents();
                Messages.Clear();
                ConnectTimeout = null;
                _registeredMessages.Clear();
                _registeredMessages = null;
                Messages = null;
                _webSocket = null;
                _pingTimer = null;
                _primus = null;
                _semaphore = null;
                _sessionId = null;
                _access = null;
                _sid = null;
                _serverAddress = null;
                Account = null;

                _disposedValue = true;
            }
        }

        ~NetworkManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}