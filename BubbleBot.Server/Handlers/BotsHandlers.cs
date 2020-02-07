using BubbleBot.Server.Clients;
using BubbleBot.Server.Clients.Accounts;
using BubbleBot.Server.Messages;
using System.Threading.Tasks;

namespace BubbleBot.Server.Handlers
{
    public static class BotsHandlers
    {

        public static Task HandleBotSelectedSuccesMessage(Client client, BotSelectedSuccesMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                if (client.Accounts.TryGetValue(message.Account, out Account account))
                {
                    account.SetInitialBotInformations(client.Informations.Id, message.Id, message.Name, message.Server, message.Level);
                    ServerMain.BroadcastStatistics();
                }
            });

        public static Task HandleBotsInformationsMessage(Client client, BotsInformationsMessage message)
            => Task.Run(() =>
            {
                if (!client.LoggedIn)
                    return;

                foreach (var kvp in message.Bots)
                {
                    if (client.Accounts.TryGetValue(kvp.Key, out Account account))
                    {
                        account.UpdateBotInformations(client.Informations.Id, kvp.Value.Level, kvp.Value.EnergyPercent, kvp.Value.WeightPercent, kvp.Value.Kamas, kvp.Value.MapId, kvp.Value.MapPosition, 
                            kvp.Value.State);
                    }
                }
            });

    }
}
