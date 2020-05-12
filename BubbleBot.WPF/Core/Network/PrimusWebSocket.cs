using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Core.Accounts;
using BubbleBot.Utility;
using BubbleBot.Utility.Security;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace BubbleBot.Core.Network
{
    public class PrimusWebSocket : IDisposable
    {
        private readonly ConcurrentQueue<JObject> _messagesQueue;
        private Timer _socketIOtimer;
        private bool _waitingToBeClosed;
        private Account _account;
  

        // Fields
        private WebSocket _webSocket;

        // Constructors
        public PrimusWebSocket(Account account)
        {
            _messagesQueue = new ConcurrentQueue<JObject>();
            _socketIOtimer = new Timer(SocketTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            _waitingToBeClosed = false;
            _account = account;
        }

        // Properties
        public Uri Url { get; private set; }
        public bool Connected { get; private set; }
        public long? SocketPingTimeout { get; set; }
        public long? SocketPingInterval { get; set; }

        public void Dispose()
        {
            _socketIOtimer.Change(Timeout.Infinite, Timeout.Infinite);
            _socketIOtimer.Dispose();
            //webSocket.Dispose(); TODO: Implement this

            Url = null;
            Connected = false;
            SocketPingInterval = null;
            SocketPingTimeout = null;
            _webSocket = null;
            _socketIOtimer = null;
            _account = null;
        }


        // Events
        public event Action<PrimusWebSocket, Exception> ErrorOccured;
        public event Action<PrimusWebSocket, JObject> MessageReceived;
        public event Action<PrimusWebSocket> Opened;
        public event Action<PrimusWebSocket> Closed;


        public async Task OpenAsync(string url, string sid, string proxyUrl = null, string proxyUsername = null,
            string proxyPassword = null)
        {
            if (_webSocket != null)
                RemoveEvents();

            if (url == null || url == "")
                return;

            try
            {
                InitializeWebsocket(url, sid, proxyUrl, proxyUsername, proxyPassword);
            }
            catch (Exception e)
            {
                ErrorOccured?.Invoke(this, e);
            }
            await Task.Delay(2000);

            await OpenAsync().ConfigureAwait(false);
        }

        public Task OpenAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    _webSocket.ConnectAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur WEBSOCKET lors de l'ouverture async");
                    ErrorOccured?.Invoke(this, e);
                }
            });
        }

        private async Task ProcessMessages()
        {
            try
            {
                while (Connected || _messagesQueue.Count > 0)
                {
                    if (_messagesQueue.Count > 0)
                    {
                        if (_messagesQueue.TryDequeue(out var obj)) MessageReceived?.Invoke(this, obj);
                    }
                    else if (_waitingToBeClosed)
                    {
                        _waitingToBeClosed = false;
                        Connected = false;
                    }

                    // Friendly Infinite Loop
                    await Task.Delay(10);
                }
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(this, ex);
            }

            Closed?.Invoke(this);
        }

        public Task CloseAsync(string reason)
        {
            return Task.Run(() =>
            {
                try
                {
                    _webSocket.Close(CloseStatusCode.Normal, reason);
                }
                catch (Exception e)
                {
                    ErrorOccured?.Invoke(this, e);
                }
            });
        }

        public Task SendAsync(string data)
        {
            return Task.Run(() =>
            {
                try
                {
                    data = $"4{data}";
                    _webSocket.Send(data);
                }
                catch (Exception e)
                {
                    ErrorOccured?.Invoke(this, e);
                }
            });
        }

        private void RemoveEvents()
        {
            _webSocket.OnOpen -= WebSocket_Opened;
            _webSocket.OnMessage -= WebSocket_MessageReceived;
            _webSocket.OnClose -= WebSocket_Closed;
            _webSocket.OnError -= WebSocket_ErrorOccured;
        }

        private void InitializeWebsocket(string url, string sid, string proxyUrl, string proxyUsername,
            string proxyPassword)
        {
            Url = new Uri(url + "&sid=" + sid + "&t=" + YeastAPI.GenerateKey() + "&b64=1");
            Console.WriteLine(Url);

            _webSocket = new WebSocket(Url.AbsoluteUri);
            _webSocket.SetCookie(new Cookie("io", sid));
            _webSocket.Compression = CompressionMethod.Deflate;

             if (proxyUrl?.Length > 0) _webSocket.SetProxy(proxyUrl, proxyUsername ?? "", proxyPassword ?? "");

            _waitingToBeClosed = false;

            _webSocket.OnOpen += WebSocket_Opened;
            _webSocket.OnMessage += WebSocket_MessageReceived;
            _webSocket.OnClose += WebSocket_Closed;
            _webSocket.OnError += WebSocket_ErrorOccured;
        }

        private void SocketTimerCallback(object state)
        {
            if (!Connected)
                return;

            _webSocket.Send("2");
        }


        private void WebSocket_ErrorOccured(object sender, ErrorEventArgs e)
        {
            ErrorOccured?.Invoke(this, e.Exception);
        }

        private void WebSocket_MessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine("data received: {0}", e.Data); //123456

            // Useless message (or not?)
            if (e.Data.Length == 0)
                return;

            if (e.Data == "3")
            {
                _socketIOtimer.Change(SocketPingInterval ?? 25000, SocketPingInterval ?? 25000);
                return;
            }

            if (e.Data[0] != '0' && e.Data[0] != '4')
                return;

            // If its a primus response
            if (e.Data.Contains("primus::ping::"))
            {
                SendAsync(e.Data.Replace("ping", "pong").Substring(1)).ConfigureAwait(false);
                return;
            }

            if (e.Data.Contains("primus::server::close"))
            {
                Console.WriteLine(e.Data);
                return;
            }

            try
            {
                var msg = e.Data.Substring(1);
                var obj = JObject.Parse(msg);

                // Handle ping => not needed anymore
                if (e.Data[0] == '0' && obj["pingInterval"] != null)
                    return;

                _messagesQueue.Enqueue(obj);
            }
            catch (Exception ex)
            {
                DebugFileWriter.WriteFile("DebugFile.txt", e.Data);
                ErrorOccured?.Invoke(this, ex);
            }
        }

        private void WebSocket_Closed(object sender, EventArgs e)
        {
            if (Connected == false)
            {
                _waitingToBeClosed = true;
                _socketIOtimer.Change(Timeout.Infinite, Timeout.Infinite);
                Console.WriteLine("NON CONNECTER ");
                _account.State = Enums.AccountStates.DISCONNECTED;
                _account.Connect().ConfigureAwait(false);
            }
            else
            {
                _waitingToBeClosed = true;
                _socketIOtimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void WebSocket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Websocket open ");

            _webSocket.Send("2probe");
            _webSocket.Send("5");

            _socketIOtimer.Change(SocketPingInterval ?? 25000, SocketPingInterval ?? 25000);

            Connected = true;


            Task.Factory.StartNew(ProcessMessages, TaskCreationOptions.LongRunning);

            Opened?.Invoke(this);
        }
    }
}