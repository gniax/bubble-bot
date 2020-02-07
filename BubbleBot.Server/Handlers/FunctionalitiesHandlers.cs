using System.Linq;
using BubbleBot.Server.Clients;
using BubbleBot.Server.Clients.Accounts;
using BubbleBot.Server.Enums;
using BubbleBot.Server.Messages;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExtensionsEnum = BubbleBot.Protocol.Server.Enums.Extensions;

namespace BubbleBot.Server.Handlers
{
    public static class FunctionalitiesHandlers
    {

        // Fields
        private static string _openBagsPattern = "OPEN_BAGS|open_bags";
        private static string _autoDeletePattern = "AUTO_DELETE|auto_delete";
        private static string _phenixFunctionPattern = @"PHENIX\(\)|phenix\(\)";
        private static string _bidPattern = @"BID\.|bid\.";
        private static string _exchangePattern = @"EXCHANGE\.|exchange\.";


        public static Task HandleShowFunctionalitiesRequestMessage(Client client, ShowFunctionalitiesRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                // Only the subscribed users (either Ruby or Emerald) can have Statistics
                if (client.Informations.IsSubscribedToTouch)
                {
                    client.SendMessage(new ShowFunctionalityMessage(Functionalities.FLOOD));
                    client.SendMessage(new ShowFunctionalityMessage(Functionalities.STATISTICS));
                }

                // Only users that have the extension can have this
                if (client.Informations.HasExtension(ExtensionsEnum.HDV))
                {
                    client.SendMessage(new ShowFunctionalityMessage(Functionalities.HDV));
                }
            });

        public static Task HandleSetProxyRequestMessage(Client client, SetProxyRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                if (client.Informations.IsSubscribedToTouch)
                {
                    client.SendMessage(new SetProxyMessage(message.Account, message.IpAddress, message.Port, message.Username, message.Password));
                }
                else
                {
                    client.SendMessage(new InvalidOperationMessage(InvalidOperations.UNAUTHORIZED_PROX_SET));
                }
            });

        public static Task HandleAddAuthorizedTradeFromRequestMessage(Client client, AddAuthorizedTradeFromRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                if (!client.Accounts.TryGetValue(message.Account, out Account account) || !account.HasBot)
                    return;

                if (client.Informations.IsSubscribedToTouch)
                {
                    client.SendMessage(new AddAuthorizedTradeFromMessage(message.Account, message.CharacterId));
                }
                else
                {
                    client.SendMessage(new InvalidOperationMessage(InvalidOperations.UNAUTHORIZED_EXCHANGES));
                }
            });

        public static Task HandleLoadScriptRequestMessage(Client client, LoadScriptRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                if (!client.Accounts.TryGetValue(message.Account, out Account account) || !account.HasBot)
                    return;

                if (!IsScriptValid(client, message.Content))
                    return;

                client.SendMessage(new LoadScriptMessage(message.Account, message.Path));
            });

        public static Task HandleStartScriptRequestMessage(Client client, StartScriptRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                if (!client.Accounts.TryGetValue(message.Account, out Account account) || !account.HasBot)
                    return;

                // In case a normal user reached his max level
                if (!client.Informations.IsSubscribedToTouch && account.BotLevel >= 9)
                {
                    client.SendMessage(new InvalidOperationMessage(InvalidOperations.MAX_LEVEL_REACHED));
                }
                else
                {
                    client.SendMessage(new StartScriptMessage(message.Account));
                }
            });

        public static Task HandleStartBidExtensionRequestMessage(Client client, StartBidExtensionRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                if (!client.Accounts.TryGetValue(message.Account, out Account account) || !account.HasBot)
                    return;

                // Only subscribed users that have the extension have the right to use this
                if (client.Informations.IsSubscribedToTouch && client.Informations.HasExtension(ExtensionsEnum.HDV))
                {
                    client.SendMessage(new StartBidExtensionMessage(message.Account));
                }
            });

        public static Task HandleStartFloodExtensionRequestMessage(Client client, StartFloodExtensionRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                if (!client.Accounts.TryGetValue(message.Account, out Account account) || !account.HasBot)
                    return;

                // Only subscribed users can use flood
                if (client.Informations.IsSubscribedToTouch)
                {
                    client.SendMessage(new StartFloodExtensionMessage(message.Account));
                }
            });

        public static Task HandleQuickActionRequestMessage(Client client, QuickActionRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                var accounts = client.Accounts.Values.Where(a => a.HasBot && message.Accounts.Contains(a.Username)).ToArray();

                switch (message.Action)
                {
                    case 0 when message.Parameters.Length == 2: // Load a script

                        if (!IsScriptValid(client, message.Parameters[1]))
                            return;

                        message.Parameters = new[] { message.Parameters[0] };

                        break;
                    case 1: // Start the script

                        if (!client.Informations.IsSubscribedToTouch && accounts.Any(a => a.BotLevel >= 9))
                        {
                            client.SendMessage(new InvalidOperationMessage(InvalidOperations.MAX_LEVEL_REACHED));
                            accounts = accounts.Where(a => a.BotLevel < 9).ToArray();
                        }

                        break;
                }

                client.SendMessage(new QuickActionMessage(accounts.Select(a => a.Username).ToArray(), message.Action, message.Parameters));
            });

        private static bool IsScriptValid(Client client, string content)
        {
            // If the user isn't subscribed, check for functionalities
            if (!client.Informations.IsSubscribedToTouch)
            {
                // OPEN_BAGS
                if (Regex.IsMatch(content, _openBagsPattern))
                {
                    client.SendMessage(new InvalidOperationMessage(InvalidOperations.UNAUTHORIZED_OPEN_BANGS));
                    return false;
                }

                // AUTO_DELETE
                if (Regex.IsMatch(content, _autoDeletePattern))
                {
                    client.SendMessage(new InvalidOperationMessage(InvalidOperations.UNAUTHORIZED_AUTO_DELETE));
                    return false;
                }

                // PHENIX
                if (Regex.IsMatch(content, _phenixFunctionPattern))
                {
                    client.SendMessage(new InvalidOperationMessage(InvalidOperations.UNAUTHORIZED_PHENIX_FUNCTION));
                    return false;
                }

                // EXCHANGE
                if (Regex.IsMatch(content, _exchangePattern))
                {
                    client.SendMessage(new InvalidOperationMessage(InvalidOperations.UNAUTHORIZED_EXCHANGES));
                    return false;
                }
            }

            // Check for Bid usage in case the user doesn't have the extension
            if (!client.Informations.HasExtension(ExtensionsEnum.HDV) && Regex.IsMatch(content, _bidPattern))
            {
                client.SendMessage(new InvalidOperationMessage(InvalidOperations.UNAUTHORIZED_BID));
                return false;
            }

            return true;
        }

    }
}
