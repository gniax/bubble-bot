using BubbleBot.Server.Clients.Accounts;
using BubbleBot.Server.Enums;
using BubbleBot.Server.Handlers;
using BubbleBot.Server.Messages;
using BubbleBot.Server.Network;
using BubbleBot.Server.Utility.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BubbleBot.Server.Clients
{
    public class Client : IDisposable
    {
        // Fields
        private ClientWrapper _clientWrapper;

        private SemaphoreSlim _semaphore;
        private Timer _botsInformationsTimer;
        private Timer _pingTimer;
        private Timer _pingTimeoutTimer;


        // Properties
        public ClientWrapper Network => _clientWrapper;

        public bool Running => Network.Running;
        public ClientInformations Informations { get; private set; }
        public bool LoggedIn { get; set; }
        public ConcurrentDictionary<string, Account> Accounts { get; private set; }


        // Constructor
        public Client(ClientWrapper clientWrapper)
        {
            _clientWrapper = clientWrapper;
            _semaphore = new SemaphoreSlim(1, 1);
            _pingTimer = new Timer(Ping_Callback, null, 30000, 30000);
            _pingTimeoutTimer = new Timer(PingTimeout_Callback, null, Timeout.Infinite, Timeout.Infinite);
            Informations = new ClientInformations(this);
            Accounts = new ConcurrentDictionary<string, Account>();

            Network.DataReceived += Network_DataReceived;
            Network.ErrorOccured += Network_ErrorOccured;
            Network.Disconnected += Network_Disconnected;
        }


        public bool AddAccounts(IEnumerable<string> usernames)
        {
            _semaphore.Wait();
            bool result = true;

            foreach (var username in usernames)
            {
                if (!Accounts.TryAdd(username, new Account(username)))
                    result = false;
            }

            _semaphore.Release();
            return result;
        }

        public async Task RemoveAccounts(IEnumerable<string> usernames, int clientid = 0)
        {
            try
            {
                bool botsCountChanged = false;

                foreach (var username in usernames)
                {
                    if (Accounts.TryRemove(username, out Account temp) && temp.HasBot)
                    {
                        if (clientid != 0 && temp.BotState != AccountStates.BANNED.ToString())
                        {
                            temp.UpdateBotInformations(clientid, temp.BotLevel, temp.BotEnergyPercent, temp.BotWeightPercent, temp.BotKamas, temp.BotMapId, temp.BotMapPosition,
                                                       AccountStates.DISCONNECTED.ToString(), "-", 0, temp.BotScriptName);

                            temp.ArchiveBotsInformations(clientid, temp.BotId, temp.BotName, temp.BotServer, temp.BotBreed, temp.BotLevel, temp.BotEnergyPercent, temp.BotWeightPercent, temp.BotKamas, temp.BotMapId, temp.BotMapPosition,
                                                       AccountStates.DISCONNECTED.ToString(), "-", 0, temp.BotScriptName);
                        }
                        else if (clientid != 0 && temp.BotState == AccountStates.BANNED.ToString())
                        {
                            temp.UpdateBotInformations(clientid, temp.BotLevel, temp.BotEnergyPercent, temp.BotWeightPercent, temp.BotKamas, temp.BotMapId, temp.BotMapPosition,
                                                       AccountStates.BANNED.ToString(), "-", 0, temp.BotScriptName);
                            temp.ArchiveBotsInformations(clientid, temp.BotId, temp.BotName, temp.BotServer, temp.BotBreed, temp.BotLevel, temp.BotEnergyPercent, temp.BotWeightPercent, temp.BotKamas, temp.BotMapId, temp.BotMapPosition,
                                 AccountStates.BANNED.ToString(), "-", 0, temp.BotScriptName);
                        }
                        botsCountChanged = true;
                    }
                }

                if (botsCountChanged)
                {
                    ServerMain.BroadcastStatistics();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur par rapport au client {0}, informations: {1}.", Informations.ToString(), ex.ToString());
            }
        }

        public void SendMessage(IServerMessage message, bool withoutMsg = false)
        {
            var bytes = new List<byte>();

            using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
            {
                writer.Write(message.MessageId);
                message.Serialize(writer);

                bytes.AddRange(writer.GetAllBytes());

                // Insert the message length in the beginning
                bytes.InsertRange(0, BitConverter.GetBytes(bytes.Count));
            }

            Network.Send(bytes.ToArray());

            if (withoutMsg)
                return;

            Console.WriteLine("{0} envoyé au client {1}.", message.GetType().Name, Informations.ToString());
        }

        #region Client events

        private void Network_DataReceived(ClientWrapper client, byte[] data)
        {
            try
            {
                // Ignore pong message
                if (data.Length == 1 && data[0] == 1)
                    return;

                var message = MessagesBuilder.BuildMessage(data);

                // If the message failed to build, do nothing
                if (message == null)
                    return;

                if (!(message is PongMessage))
                    Console.WriteLine("Received {0} from client {1}.", message.GetType().Name, Informations.ToString());

                HandlersManager.HandleMessage(this, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured with client {0}, message: {1}.", Informations.ToString(),
                    ex.ToString());
            }
        }

        private void Network_ErrorOccured(ClientWrapper client, Exception exception)
        {
            Console.WriteLine("Exception occured in client {0}, informations: {1}.", Informations.ToString(),
                exception.ToString());
        }

        private void Network_Disconnected(ClientWrapper client)
        {
            LoggedIn = false;
        }

        #endregion

        #region Ping/Pong

        private void Ping_Callback(object state)
        {
            SendMessage(new PingMessage(), true);
            _pingTimeoutTimer.Change(5000, Timeout.Infinite);
        }

        private void PingTimeout_Callback(object state)
        {

            if (Running)
            {
                Console.WriteLine("Client {0} timed out.", Informations);
                StopPingTimeoutTimer();
                Network.Close();
            }
            else
            {
                Console.WriteLine("Client {0} timed out and already disconnected.", Informations);
                ServerMain.RemoveClient(Informations.Id);

            }
        }

        public void StopPingTimeoutTimer()
            => _pingTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);

        #endregion

        #region Dispose

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _semaphore?.Dispose();
            _botsInformationsTimer?.Dispose();
            Informations?.Dispose();
            Accounts?.Clear();
            _pingTimer?.Dispose();
            _pingTimeoutTimer?.Dispose();

            _semaphore = null;
            _botsInformationsTimer = null;
            Informations = null;
            LoggedIn = false;
            Accounts = null;
            _clientWrapper = null;
            _pingTimer = null;
            _pingTimeoutTimer = null;

            _disposed = true;
        }

        #endregion

    }
}