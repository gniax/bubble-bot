using System;
using System.Linq;
using System.Text;

namespace BubbleBot.Server.Commands
{
    public static class ModerationCommands
    {
        [Command("help")]
        public static void HelpCommand(string[] args)
        {
            if (args.Length != 0)
                return;

            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Red; Console.Write("/removeClient"); Console.ForegroundColor = ConsoleColor.DarkBlue; Console.Write(" {clientID} ");
            Console.ResetColor(); Console.Write("=> Permet d'éjecter un client du serveur. \n");
            Console.ForegroundColor = ConsoleColor.Red; Console.Write("/clientInfos"); Console.ForegroundColor = ConsoleColor.DarkBlue; Console.Write(" {clientID} ");
            Console.ResetColor(); Console.Write("=> Affiche les informations du client. \n");
            Console.ForegroundColor = ConsoleColor.Red; Console.Write("/refreshFilesHashes ");
            Console.ResetColor(); Console.Write("=> Actualise les data des fichiers du jeu. \n");
            Console.ForegroundColor = ConsoleColor.Red; Console.Write("/setDTVersions ");
            Console.ResetColor(); Console.Write("=> Actualise les versions du jeu. \n");
            Console.WriteLine("-------------------------------------------------------------------------------");

        }

        [Command("removeClient")]
        public static void RemoveClientCommand(string[] args)
        {
            if (args.Length != 1)
                return;

            if (int.TryParse(args[0], out int clientId))
            {
                ServerMain.RemoveClient(clientId);
            }
        }

        [Command("clientInfos")]
        public static void ClientInfosCommand(string[] args)
        {
            if (args.Length != 1)
                return;

            if (!int.TryParse(args[0], out int clientId))
                return;

            var client = ServerMain.Clients.FirstOrDefault(c => c.Informations.Id == clientId);

            if (client == null)
                return;

            var sb = new StringBuilder();

            sb.AppendLine($"Client: {client.Informations}");
            sb.AppendLine($"Accounts: {client.Accounts.Count}");
            sb.AppendLine($"Bots: {client.Accounts.Values.Count(a => a.HasBot)}");

            Console.WriteLine(sb.ToString());
        }

    }
}
