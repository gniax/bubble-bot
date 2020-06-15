using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Enums;
using BubbleBot.Core.Groups;
using BubbleBot.Server.Messages;
using BubbleBot.Server.Network;
using BubbleBot.Utility.DofusTouch;
using BubbleBot.Utility.Extensions;
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ExtensionsEnum = BubbleBot.Protocol.Server.Enums.Extensions;

namespace BubbleBot.Server
{
    public class ServerManager : ViewModelBase
    {
        private readonly ConcurrentDictionary<Type, List<Action<object>>> _registeredMessages;
        private string _avatarUrl;

        // Fields
        private ClientWrapper _client;
        private LoginRequestMessage _lastLrm;
        private string _name;
        private ServerConnectionStates _state;
        private DateTime? _touchEndDate;


        // Constructor
        public ServerManager()
        {
            _client = new ClientWrapper();
            _registeredMessages = new ConcurrentDictionary<Type, List<Action<object>>>();
            State = ServerConnectionStates.DISCONNECTED;
            Statistics = new ServerStatistics(this);
            Extensions = new Dictionary<ExtensionsEnum, DateTime>();

            ReconnectionSuccess += Client_Reconnected;
            _client.Connected += Client_Connected;
            _client.DataReceived += Client_DataReceived;
            _client.ErrorOccured += Client_ErrorOccured;
            _client.Disconnected += Client_Disconnected;

            RegisterMessage<LoginAcceptedMessage>(HandleLoginAcceptedMessage);
            RegisterMessage<SubscriptionInformationsMessage>(HandleSubscriptionInformationsMessage);
            RegisterMessage<BotsInformationsRequestMessage>(HandleBotsInformationsRequestMessage);
            RegisterMessage<DTVersionsMessage>(HandleDTVersionsMessage);
            RegisterMessage<PingMessage>(HandlePingMessage);
            RegisterMessage<ReconnectSuccessMessage>(HandleReconnectSuccessMessage);
        }


        // Properties
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string AvatarUrl
        {
            get => _avatarUrl;
            set => Set(ref _avatarUrl, value);
        }

        public ServerConnectionStates State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        public bool LoggedIn { get; private set; }

        public DateTime? TouchEndDate
        {
            get => _touchEndDate;
            set
            {
                _touchEndDate = value;
                RaisePropertyChanged();
            }
        }

        public Dictionary<ExtensionsEnum, DateTime> Extensions { get; private set; }
        public ServerStatistics Statistics { get; }

        public bool IsSubscribedToTouch => TouchEndDate != null && DateTime.Now < TouchEndDate;


        // Events
        public event Action LoginAccepted;
        public event Action<ClientWrapper> ReconnectionSuccess;


        public void Start()
        {
            if (_client.Running)
                return;

            Task.Delay(3000);
            _client.Connect(Constants.ServerHost, Constants.ServerService);
            FunctionalitiesManager.Initialize();
        }

        public void RegisterMessage<T>(Action<T> handler) where T : IServerMessage
        {
            var msgType = typeof(T);
            if (!_registeredMessages.ContainsKey(msgType))
                _registeredMessages.TryAdd(msgType, new List<Action<object>>());

            _registeredMessages[msgType].Add(m => handler((T) m));
        }

        public void SendMessage(IServerMessage message)
        {
            if (State != ServerConnectionStates.CONNECTED)
                return;

            var bytes = new List<byte>();

            using (var writer = new BinaryWriter(new MemoryStream()))
            {
                writer.Write(message.MessageId);
                message.Serialize(writer);

                bytes.AddRange(writer.GetAllBytes());

                // Insert the message length in the beginning
                bytes.InsertRange(0, BitConverter.GetBytes(bytes.Count));
            }

            _client.Send(bytes.ToArray());

            // Save the last LoginRequestMessage in case we need to reconnect
            if (message is LoginRequestMessage lrm) _lastLrm = lrm;
        }

        public bool HasExtension(ExtensionsEnum extension)
        {
            return Extensions?.ContainsKey(extension) == true && Extensions?[extension] > DateTime.Now;
        }

        #region Client events

        private void Client_Connected(ClientWrapper client)
        {
            State = ServerConnectionStates.CONNECTED;
        }

        private void Client_DataReceived(ClientWrapper client, byte[] data)
        {
            try
            {
                if (State != ServerConnectionStates.CONNECTED)
                    return;

                var message = MessagesBuilder.BuildMessage(data);

                // If the message failed to build, do nothing
                if (message == null)
                    return;

                Console.WriteLine("Received {0}.", message.GetType().Name);

                var type = message.GetType();
                if (!_registeredMessages.ContainsKey(type)) return;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var action in _registeredMessages[type]) action.Invoke(message);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void Client_ErrorOccured(ClientWrapper client, Exception exception)
        {
            // If the connection to the server fails before we even log in, it probably means that the server is having a tough time
            if (!LoggedIn && exception is SocketException se &&
                (se.SocketErrorCode == SocketError.ConnectionRefused || se.SocketErrorCode == SocketError.TimedOut))
            {
                try
                {
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await (Application.Current.MainWindow as MetroWindow).ShowMessageAsync(
                            LanguageManager.Translate("249"), LanguageManager.Translate("484"));
                    });
                }
                catch
                {
                }

                Environment.Exit(0);
            }
        }

        private void Client_Reconnected(ClientWrapper client)
        {
            Task.Run(() =>
            {
                BubbleBotMain.Instance.Server._client = client;
                Task.Delay(2000).Wait();
                BubbleBotMain.Instance.Server.SendMessage(new ReconnectRequestMessage(_lastLrm.Username,
                    _lastLrm.Password));
            }).ConfigureAwait(false);
        }

        private void Client_Disconnected(ClientWrapper client)
        {
            State = ServerConnectionStates.DISCONNECTED;
            while (true)
            {
                _client.Close();
                _client.Dispose();
                Task.Delay(2000);
                _client = new ClientWrapper();

                State = ServerConnectionStates.DISCONNECTED;
                Extensions = new Dictionary<ExtensionsEnum, DateTime>();

                _client.Connected += Client_Connected;
                _client.DataReceived += Client_DataReceived;
                _client.ErrorOccured += Client_ErrorOccured;
                _client.Disconnected += Client_Disconnected;

                _client.Connect(Constants.ServerHost, Constants.ServerService);
                var result = SpinWait.SpinUntil(() => State == ServerConnectionStates.CONNECTED, 10000);
                if (result)
                {
                    ReconnectionSuccess?.Invoke(_client);
                    break;
                }
            }
        }

        #endregion

        #region Server messages

        private static void HandleReconnectSuccessMessage(ReconnectSuccessMessage message)
        {
            if (BubbleBotMain.Instance.Entities.Count > 0)
                foreach (var account in BubbleBotMain.Instance.EveryConnectedAccount())
                {
                    BubbleBotMain.Instance.Server.SendMessage(
                        new ConnectedAccountMessage(account.AccountConfig.Username));
                    if (account.Game.Character.IsSelected)
                        BubbleBotMain.Instance.Server.SendMessage(new BotSelectedSuccessMessage(
                            account.AccountConfig.Username, (int) account.Game.Character.Id,
                            account.Game.Character.Name,
                            account.Game.Server.Name, account.Game.Character.Breed.ToString(),
                            account.Game.Character.Level));
                }
        }

        private void HandleLoginAcceptedMessage(LoginAcceptedMessage message)
        {
            Name = message.Name;
            AvatarUrl = message.Avatar;
            LoggedIn = true;

            LoginAccepted?.Invoke();
        }

        private void HandleSubscriptionInformationsMessage(SubscriptionInformationsMessage message)
        {
            TouchEndDate = message.TouchEndDate;
            Extensions = message.Extensions;
        }

        private void HandleBotsInformationsRequestMessage(BotsInformationsRequestMessage message)
        {
            bool TryGenerateBot(Account account, out Bot bot)
            {
                bot = null;

                try
                {
                    if (account.Game.Character.IsSelected && account.State != AccountStates.DISCONNECTED &&
                        account.State != AccountStates.BANNED)
                    {
                        bot = new Bot
                        (
                            account.Game.Character.Level,
                            (byte) account.Game.Character.Stats.EnergyPercent,
                            (byte) account.Game.Character.Inventory.WeightPercent,
                            account.Game.Character.Inventory.Kamas,
                            account.Game.Map.Id,
                            account.Game.Map.CurrentPosition,
                            account.State.ToString(),
                            account.GroupId,
                            account.Group_Chief,
                            account.Scripts.CurrentScriptName != null ? account.Scripts.CurrentScriptName : "-"
                        );

                        return true;
                    }
                }
                catch
                {
                }

                return false;
            }

            var bots = new Dictionary<string, Bot>();

            foreach (var entity in BubbleBotMain.Instance.Entities)
                switch (entity)
                {
                    case Account account:
                    {
                        if (TryGenerateBot(account, out var bot)) bots.Add(account.AccountConfig.Username, bot);
                        break;
                    }
                    case Group group:
                    {
                        if (TryGenerateBot(group.Chief, out var bot)) bots.Add(group.Chief.AccountConfig.Username, bot);

                        for (var i = 0; i < group.Members.Count; i++)
                            if (TryGenerateBot(group.Members[i], out var mbot))
                                bots.Add(group.Members[i].AccountConfig.Username, mbot);
                        break;
                    }
                }

            SendMessage(new BotsInformationsMessage(bots));

            // Here we check if there are no UnknowEntities to notify the server
        }

        private void HandleDTVersionsMessage(DTVersionsMessage message)
        {
            //Console.WriteLine("{0} {1} {2} {3}", message.AppVersion, message.BuildVersion, message.AssetsVersion, message.StaticDataVersion);
            DTConstants.AppVersion = message.AppVersion;
            DTConstants.BuildVersion = message.BuildVersion;
            DTConstants.AssetsVersion = message.AssetsVersion;
            DTConstants.StaticDataVersion = message.StaticDataVersion;

            if (message.IsFromUpdate) // In the case the message come from an update from the server
            {
                foreach (var account in BubbleBotMain.Instance.EveryConnectedAccount())
                {
                    if (account.Network.Connected && account.Game.Character.IsSelected)
                    {
                        account.Logger.LogWarning("", LanguageManager.Translate("733"));
                    }
                }
            }
            BubbleBot.Core.Frames.Connection.IdentificationFrame.WarnServerVersionsLocker = false;
        }

        private void HandlePingMessage(PingMessage message)
        {
            SendMessage(new PongMessage());
        }

        #endregion
    }
}