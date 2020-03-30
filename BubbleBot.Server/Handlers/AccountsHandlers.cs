using BubbleBot.Server.Clients;
using BubbleBot.Server.Enums;
using BubbleBot.Server.Messages;
using System.Linq;
using System.Threading.Tasks;

namespace BubbleBot.Server.Handlers
{
    public static class AccountsHandlers
    {

        public static Task HandleConnectAccountRequestMessage(Client client, ConnectAccountRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                // Check if the user can actually connect one more account
                if (ServerMain.GetClientBotsCount(client.Informations.Name) >= client.Informations.MaxAccounts)
                {
                    client.SendMessage(new InvalidOperationMessage(InvalidOperations.MAX_ACCOUNTS_REACHED));
                    return;
                }

                // We will only send a ConnectAccountMessage if we actually added the account to the client's Accounts
                if (client.AddAccounts(new[] { message.Username }))
                {
                    client.SendMessage(new ConnectAccountMessage(message.Username));
                }
            });

        public static Task HandleConnectAccountsRequestMessage(Client client, ConnectAccountsRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                // Check how many accounts the user is actually allowed to connect
                int connectableAccountsCount = client.Informations.MaxAccounts - ServerMain.GetClientBotsCount(client.Informations.Name);
                connectableAccountsCount = connectableAccountsCount > message.Usernames.Count ? message.Usernames.Count : connectableAccountsCount;

                // If we can connect at least 1 account
                if (connectableAccountsCount > 0)
                {
                    var accountsToConnect = message.Usernames.Take(connectableAccountsCount);

                    if (client.AddAccounts(accountsToConnect))
                    {
                        client.SendMessage(new ConnectAccountsMessage(accountsToConnect.ToList()));
                    }

                    // If we connected all the accounts the user requested, return to avoid sending an InvalidOperationMessage
                    if (connectableAccountsCount == message.Usernames.Count)
                        return;
                }

                // Send an invelid operation in case the user can't connect anymore accounts
                client.SendMessage(new InvalidOperationMessage(InvalidOperations.MAX_ACCOUNTS_REACHED));
            });

        public static Task HandleConnectedAccountMessage(Client client, ConnectedAccountMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                // Check if the user can actually connect one more account
                if (ServerMain.GetClientBotsCount(client.Informations.Name) >= client.Informations.MaxAccounts)
                {
                    client.SendMessage(new InvalidOperationMessage(InvalidOperations.MAX_ACCOUNTS_REACHED));
                    return;
                }

                client.AddAccounts(new[] { message.Username });
            });

        public static Task HandleRemoveAccountRequestMessage(Client client, RemoveAccountRequestMessage message)
            => Task.Run(async () =>
            {
                if (!client.LoggedIn)
                    return;

                await client.RemoveAccounts(new[] { message.Username }, client.Informations.Id);
            });

        public static Task HandleRemoveAccountsRequestMessage(Client client, RemoveAccountsRequestMessage message)
            => Task.Run(async () =>
            {
                if (!client.LoggedIn)
                    return;

                await client.RemoveAccounts(message.Usernames, client.Informations.Id);
            });

        public static Task HandleConnectGroupRequestMessage(Client client, ConnectGroupRequestMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                // Check how many accounts the user is actually allowed to connect
                int connectableAccountsCount = client.Informations.MaxAccounts - ServerMain.GetClientBotsCount(client.Informations.Name);

                // If the user can't connect the whole group
                if (connectableAccountsCount < message.Usernames.Count)
                {
                    client.SendMessage(new InvalidOperationMessage(InvalidOperations.MAX_ACCOUNTS_REACHED));
                    return;
                }

                if (client.AddAccounts(message.Usernames))
                {
                    client.SendMessage(new ConnectGroupMessage(message.Usernames));
                }

            });

    }
}
