using System;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using WebSocketSharp;

namespace BubbleBot.Core.Network
{
    public class PrimusWebSocket : IDisposable
    {

        // Properties
        public Uri Url { get; private set; }
        public bool Connected { get; private set; }
        public long? SocketPingTimeout { get; set; }
        public long? SocketPingInterval { get; set; }

        // Fields
        private WebSocket _webSocket;
        private Timer _socketTimer;
        private ConcurrentQueue<JObject> _messagesQueue;
        private bool _waitingToBeClosed;


        // Events
        public event Action<PrimusWebSocket, Exception> ErrorOccured;
        public event Action<PrimusWebSocket, JObject> MessageReceived;
        public event Action<PrimusWebSocket> Opened;
        public event Action<PrimusWebSocket> Closed;


        // Constructors
        public PrimusWebSocket()
        {
            _messagesQueue = new ConcurrentQueue<JObject>();
            _socketTimer = new Timer(SocketTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            _waitingToBeClosed = false;
        }


        public async Task OpenAsync(string url, string sid, string proxyUrl = null, string proxyUsername = null, string proxyPassword = null)
        {
            if (_webSocket != null)
                RemoveEvents();

            try
            {
                InitializeWebsocket(url, sid, proxyUrl, proxyUsername, proxyPassword);
            }
            catch (Exception e)
            {
                ErrorOccured?.Invoke(this, e);
            }

            await OpenAsync().ConfigureAwait(false);
        }

        public Task OpenAsync()
            => Task.Run(() =>
            {
                try
                {
                    _webSocket.ConnectAsync();
                }
                catch (Exception e)
                {
                    ErrorOccured?.Invoke(this, e);
                }
            });

        private async Task ProcessMessages()
        {
            try
            {
                while (Connected || _messagesQueue.Count > 0)
                {
                    if (_messagesQueue.Count > 0)
                    {
                        if (_messagesQueue.TryDequeue(out JObject obj))
                        {
                            MessageReceived?.Invoke(this, obj);
                        }
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
            => Task.Run(() =>
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

        public Task SendAsync(string data)
            => Task.Run(() =>
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

        private void RemoveEvents()
        {
            _webSocket.OnOpen -= WebSocket_Opened;
            _webSocket.OnMessage -= WebSocket_MessageReceived;
            _webSocket.OnClose -= WebSocket_Closed;
            _webSocket.OnError -= WebSocket_ErrorOccured;
        }

        private void InitializeWebsocket(string url, string sid, string proxyUrl, string proxyUsername, string proxyPassword)
        {
            string fullUrl = url + "&sid=" + sid + "&t=" + Utility.Security.YeastAPI.GenerateKey() + "&b64=1";

            Url = new Uri(fullUrl);
            _webSocket = new WebSocket(Url.AbsoluteUri);
            _webSocket.SetCookie(new WebSocketSharp.Net.Cookie("io", sid));

            if (proxyUrl?.Length > 0)
            {
                _webSocket.SetProxy(proxyUrl, proxyPassword ?? "", proxyUsername ?? "");
            }

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


        private void WebSocket_ErrorOccured(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            ErrorOccured?.Invoke(this, e.Exception);
        }


        private void WebSocket_MessageReceived(object sender, MessageEventArgs e)
        {
            //Console.WriteLine("data received: {0}", e.Data); //123456

            // Useless message (or not?)
            if (e.Data.Length == 0)
                return;
            
            if (e.Data == "3")
            {
                _socketTimer.Change(SocketPingInterval ?? 25000, SocketPingInterval ?? 25000);
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
                ErrorOccured?.Invoke(this, ex);
            }
        }

        private void WebSocket_Closed(object sender, EventArgs e)
        {
            _waitingToBeClosed = true;
            _socketTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void WebSocket_Opened(object sender, EventArgs e)
        {
            _webSocket.Send("2probe");
            _webSocket.Send("5");

            _socketTimer.Change(SocketPingInterval ?? 25000, SocketPingInterval ?? 25000);

            Connected = true;


            Task.Factory.StartNew(ProcessMessages, TaskCreationOptions.LongRunning);

            Opened?.Invoke(this);
        }

        public void Dispose()
        {
            _socketTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _socketTimer.Dispose();
            //webSocket.Dispose(); TODO: Implement this

            Url = null;
            Connected = false;
            SocketPingInterval = null;
            SocketPingTimeout = null;
            _webSocket = null;
            _socketTimer = null;
        }

    }
}