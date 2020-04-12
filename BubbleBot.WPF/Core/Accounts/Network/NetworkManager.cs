using BubbleBot.Configurations;
using BubbleBot.Core.Enums;
using BubbleBot.Core.Frames;
using BubbleBot.Core.Network;
using BubbleBot.Protocol.Messages;
using BubbleBot.Utility.DofusTouch;
using BubbleBot.Utility.Extensions;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Network
{
    public class NetworkManager : ViewModelBase, IClearable, IDisposable
    {

        // Fields
        private static readonly List<string> MessagesToIgnore = new List<string>
        {
            "ChatServerMessage", "BasicLatencyStatsRequestMessage", "BasicLatencyStatsMessage", "SequenceNumberRequestMessage", "SequenceNumberMessage", "BasicPingMessage"
        };
        private NetworkPhases _phase;
        private ConcurrentDictionary<string, RegisteredMessage> _registeredMessages;
        private PrimusWebSocket _webSocket;
        private Timer _pingTimer;
        private SemaphoreSlim _semaphore;
        private string _sessionId;
        private string _sid;
        private int _serverId;
        private string _serverAddress;
        private uint _serverPort;
        private string _primus;
        private string _access;

        public Timer ConnectTimeout;

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


        // Events
        public event Action<NetworkManager> PhaseChanged;
        public event Action<NetworkManager> Disconnected;


        // Constructor
        public NetworkManager(Account account)
        {
            Account = account;
            _phase = NetworkPhases.NONE;
            _webSocket = new PrimusWebSocket();
            _pingTimer = new System.Threading.Timer(PingTimerCallback, null, 600000, 600000);
            _semaphore = new SemaphoreSlim(1);
            _registeredMessages = new ConcurrentDictionary<string, RegisteredMessage>();
            Messages = new List<NetworkMessage>();

            AddEvents();
        }


        public async Task ConnectToLoginServer()
        {
            if (Connected)
                return;

            if (Phase != NetworkPhases.NONE)
                Phase = NetworkPhases.NONE;

            _sessionId = 16.ToRandomString();
            _primus = Utility.Security.YeastAPI.GenerateKey();

            ConnectTimeout = new Timer(ConnectTimeoutCallback, null, Timeout.Infinite, Timeout.Infinite);

            // Url as null if it is the first time then we use the selected server as url
            if (!await SetSid(null, _sessionId, Account.AccountConfig.Proxy.Ip ?? "", Account.AccountConfig.Proxy.Port.ToString(), Account.AccountConfig.Proxy.Username ?? "",
                Account.AccountConfig.Proxy.Password ?? ""))
            {
                return;
            }

            await _webSocket.OpenAsync($"wss://proxyconnection.touch.dofus.com/primus/?STICKER={_sessionId}&_primuscb={_primus}&EIO=3&transport=websocket", _sid,
                Account.AccountConfig.Proxy.Url ?? null, Account.AccountConfig.Proxy.Username ?? null, Account.AccountConfig.Proxy.Password ?? null);
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
            _access = $"{access.Replace("https", "wss")}/primus/?STICKER={_sessionId}&_primuscb={_primus}&EIO=3&transport=websocket";
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
                if (unintentional == false)
                {
                    Account.IsIntentionalDisconnection = true;
                }

                await _webSocket.CloseAsync(reason).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Disconnection exception: {0}", ex.Message);
            }
        }

        #region Send Messages

        public async Task SendRawAsync(string text)
        {
            if (!Connected)
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
            if (!MessagesToIgnore.Contains(message.GetType().Name))
            {
                AddMessage(json, true);
            }

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
            if (!MessagesToIgnore.Contains(message.GetType().Name))
            {
                AddMessage(json, true);
            }

            _semaphore.Release();
        }

        public void SendMessage(Message message)
            => SendMessageAsync(message).Wait();

        #endregion

        public string RegisterMessage<T>(Func<Account, T, Task> action) where T : Message
        {
            if (_disposedValue)
                return null;

            Task NewAction(Account a, object o) => action(a, (T)o);
            string id = 16.ToRandomString();
            _registeredMessages.TryAdd(id, new RegisteredMessage(typeof(T), NewAction));

            return id;
        }

        public bool UnregisterMessage(string id)
            => _registeredMessages.TryRemove(id, out RegisteredMessage rm);

        public void Clear()
        {
            _sessionId = null;
            _serverId = 0;
            _serverAddress = null;
            _serverPort = 0;
            _primus = null;
            _access = null;
            _sid = null;
            if(ConnectTimeout != null)
                ConnectTimeout.Change(Timeout.Infinite, Timeout.Infinite);
            ConnectTimeout = null;
        }

        private async void PingTimerCallback(object state)
        {
            if (Connected)
            {
                await Account.Network.SendMessageAsync(new BasicPingMessage(true));
            }
            else
            {
                _pingTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private async void ConnectTimeoutCallback(object state)
        {
            if (!Account.Game.Character.IsSelected || Account.PreventAutoReconnection)
            {
                Console.WriteLine("a");
                _phase = NetworkPhases.NONE;
                try
                {
                    await _webSocket.CloseAsync("CLIENT_CLOSING").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occured in PWS while closing (already closed?): {0}", ex.Message);
                    Account.Network.Disconnected?.Invoke(this);
                }
                Account.IsIntentionalDisconnection = false;
                Account.Network.Clear();
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

        #region Received Events

        private void WebSocket_ErrorOccured(PrimusWebSocket ws, Exception ex)
        {
            if (ex == null)
                return;
            Console.WriteLine("websocket-error: {0}", ex.ToString());
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
                if (!MessagesToIgnore.Contains(messageType))
                {
                    AddMessage(json.ToString(Formatting.None), false);
                }


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
            { Console.WriteLine("Exception : {0}", ex.Message); }
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
                    if (!await SetSid(_access, _sessionId, Account.AccountConfig.Proxy.Ip ?? "", Account.AccountConfig.Proxy.Port.ToString(), Account.AccountConfig.Proxy.Username ?? "",
                        Account.AccountConfig.Proxy.Password ?? ""))
                    {
                        Phase = NetworkPhases.NONE;
                        Disconnected?.Invoke(this);
                        return;
                    }

                    // Connecting to the game server
                    if (Account.AccountConfig.Proxy.IsValid)
                        await _webSocket.OpenAsync(_access, _sid, Account.AccountConfig.Proxy.Url, Account.AccountConfig.Proxy.Username, Account.AccountConfig.Proxy.Password);
                    else
                        await _webSocket.OpenAsync(_access, _sid);

                    Account.State = AccountStates.CONNECTING;
                }
                else
                {
                    Phase = NetworkPhases.NONE;
                }
            }
            catch { }
        }

        private async void WebSocket_Opened(PrimusWebSocket ws)
        {
            Connected = true;

            // Connecting to the login server
            if (Phase == NetworkPhases.NONE)
            {
                var cm = new ConnectingMessage(DTConstants.AppVersion, DTConstants.BuildVersion, GlobalConfiguration.Instance.Lang, "login", "android");
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

        private async Task<bool> SetSid(string url = null, string sticker = null, string host = "", string service = "0", string username = "", string password = "")
        {
            string yeastValue = Utility.Security.YeastAPI.GenerateKey();
            string fullUrl;
            if (url == null)
            {
                fullUrl = "https://proxyconnection.touch.dofus.com/primus/?STICKER=" + sticker + "&_primuscb=" + _primus + "&EIO=3&transport=polling&t=" + yeastValue + "&b64=1";
            }
            else
            {
                string tempUrl = url.Substring(0, url.LastIndexOf('&')) + "&_primuscb=" + _primus + "&EIO=3&transport=polling&t=" + yeastValue + "&b64=1";
                fullUrl = tempUrl.Replace("wss", "https");
            }

            HttpClient client;
            if (host != "" && service != "0")
            {
                // First create a proxy object
                WebProxy proxy = new WebProxy
                {
                    Address = new Uri($"http://{host}:{service}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false
                };

                if (service != "" && username != "")
                {
                    proxy.Credentials = new NetworkCredential()
                    {
                        UserName = username,
                        Password = password
                    };
                }
                // Now create a client handler which uses that proxy
                HttpClientHandler httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                };

                client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
            }
            else
            {
                client = new HttpClient();
            }

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Linux; Android 7.1.1; ONEPLUS A" + Utility.Randomize.GetRandomInt(1, 10000).ToString() + "Build/NMF26F; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/59.0.3071.92 Mobile Safari/537.36");

            HttpResponseMessage response = await client.GetAsync(fullUrl);

            if (response.IsSuccessStatusCode)
            {
                string result = null;
                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }

                Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(result).Substring(result.IndexOf('{')));
                _sid = (string)dictionaryRes["sid"];
                _webSocket.SocketPingInterval = (long)dictionaryRes["pingInterval"];
                _webSocket.SocketPingTimeout = (long)dictionaryRes["pingTimeout"];
                response.Dispose();
                client.Dispose();
                return true;
            }

            response.Dispose();
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
                    ConnectTimeout.Change(Timeout.Infinite, Timeout.Infinite);

                if (disposing)
                {
                    ConnectTimeout.Dispose();
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

        ~NetworkManager() => Dispose(false);

        public void Dispose() => Dispose(true);

        #endregion

    }
}