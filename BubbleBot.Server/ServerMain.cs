using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using BubbleBot.Server.Handlers;
using System.Net.Sockets;
using System.Threading;
using BubbleBot.Server.Utility;
using System.Collections.Concurrent;
using BubbleBot.Server.Clients;
using BubbleBot.Server.Commands;
using BubbleBot.Server.Network;
using BubbleBot.Server.Messages;

namespace BubbleBot.Server
{
    public static class Constants
    {
        // Server Infos
        public const int Port = 3000;

        public const string IP = "127.0.0.1"; // Server IP
        public const string ApiIpAddress = "http://localhost:5001"; // API IP

        //public const string IP = "93.113.207.95"; // Server IP
        //public const string ApiIpAddress = "http://93.113.207.95:5001"; // API IP 

        // Dofus Touch
        public static string AppVersion { get; set; } = "0.0.0";
        public static string BuildVersion { get; set; } = "0.00.0";
        public static string AssetsVersion { get; set; } = "0.00.0";
        public static string StaticDataVersion { get; set; } = "0.0.0";

        // Updates
        public static Dictionary<string, string> FilesHashes { get; set; }

        
    }
    public static class ServerMain
    { 
        public static List<Client> Clients { get; private set; }
        static void Main(string[] args)
        {
            Clients = new List<Client>();
            IPAddress ip = IPAddress.Parse(Constants.IP);
            ServerWrapper server = new ServerWrapper(ip, Constants.Port);
            server.ClientConnected += Server_ClientConnected;
            server.ErrorOccured += Server_ErrorOccured;
            server.ClientDisconnected += Server_ClientDisconnected;

            ConsoleLogger(1);  
            CommandsManager.Initialize();
            ConstantsCommands.RefreshFilesHashesCommand(null);
            StatisticsManager.Initialize();
            
            ConsoleLogger(2);  

            bool result = false;
            if (SetVersions.setVersions())
                result = true;

            ConsoleLogger(3, result);
            result = false;

            server.Start();
            if (server.RemoteIP != null)
                result = true;

            ConsoleLogger(4, result);


            // Commands
            string command;
            while ((command = Console.ReadLine()) != "exit")
            {
                if (command.StartsWith('/'))
                {
                    CommandsManager.HandleCommand(command);
                }
            }

        }
        private static void Server_ErrorOccured(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }
        private static void Server_ClientConnected(ClientWrapper client)
        {
            Clients.Add(new Client(client));
            Console.WriteLine("[+][{0}] Client connecté : {1}", DateTime.Now.ToString("HH:mm:ss"), client._ip);
        }

        private static async void Server_ClientDisconnected(ClientWrapper handler)
        {
            var client = Clients.FirstOrDefault(c => c.Network == handler);

            if (client == null)
                return;

            Console.WriteLine("[-][{0}]  Client déconnecté : {1}, {2} restant(s)", DateTime.Now.ToString("HH:mm:ss"), client.Informations.ToString(), (Clients.Count)-1);

            // Remove any accounts left
            await client.RemoveAccounts(client.Accounts.Values.Where(a => a.HasBot).Select(a => a.Username), client.Informations.Id);

            Clients.Remove(client);
            // Dispose the client and send statistics
            client.Dispose();
            BroadcastStatistics();
        }

        public static void RemoveClient(int id)
        {
            try
            {
                var client = Clients.FirstOrDefault(c => c.Informations.Id == id);

                if (client == null)
                {
                    Console.WriteLine("[?] Client introuvable.");
                    return;
                }

                // Disconnect the client if needed
                if (client.Running)
                {
                    client.Network.Close();
                }

                Clients.Remove(client);
                Console.WriteLine("[-] Client {0} supprimé, {1} restant(s).", client.Informations.ToString(), Clients.Count);

                // Remove any accounts left
                try
                {
                    client.RemoveAccounts(client.Accounts.Values.Where(a => a.HasBot).Select(a => a.Username), client.Informations.Id).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur durant la suppresion du client restant {0}, informations: {1}", client.Informations.ToString(), ex.ToString());
                }

                // Dispose the client and send statistics
                client.Dispose();
                BroadcastStatistics();
            }
            catch { }
        }

        public static void BroadcastMessage(IServerMessage message, bool onlyLoggedIn, bool withoutMsg = false)
        {
            for (int i = 0; i < Clients.Count; i++)
            {
                if (onlyLoggedIn && !Clients[i].LoggedIn)
                    continue;

                Clients[i].SendMessage(message, withoutMsg);
            }
        }

        public static void BroadcastStatistics()
            => BroadcastMessage(new ServerStatisticsMessage(Clients.Count(c => c.LoggedIn), Clients.Sum(c => c.Accounts.Values.Count(a => a.HasBot))), true, true);

        public static int GetClientInstancesCount(string username)
            => Clients.Count(c => c.LoggedIn && c.Informations.Name == username);

        public static int GetClientBotsCount(string username)
            => Clients.Where(c => c.LoggedIn && c.Informations.Name == username).Sum(c => c.Accounts.Count);

        public static readonly Spinner spinner = new Spinner(150);
        private static void ConsoleLogger(int step, bool result = false)
        {

            switch (step)
            {
                case 1:
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                    Console.WriteLine("-----------------------------------------------------");
                    Console.Write("[1/3] - Initialisation des outils Serveur");
                    spinner.Start("", 5);
                    break;

                case 2:
                    spinner.Stop();
                    ConsoleTools.ConsoleWriteSuccess();
                    Console.Write("[2/3] - Récupération des versions de Touch");
                    spinner.Start("", 5);
                    break;

                case 3:
                    spinner.Stop();
                    if (result)
                        ConsoleTools.ConsoleWriteSuccess();
                    else
                        ConsoleTools.ConsoleWriteError();
                    Console.Write("[3/3] - Démarrage du Serveur " + Constants.IP + ":" + Constants.Port);
                    spinner.Start("", 5);
                    break;

                case 4:
                    spinner.Stop();
                    if (result)
                        ConsoleTools.ConsoleWriteSuccess();
                    else
                        ConsoleTools.ConsoleWriteError();
                    Console.WriteLine("-----------------------------------------------------\n");
                    break;
            }
        }
    }
}