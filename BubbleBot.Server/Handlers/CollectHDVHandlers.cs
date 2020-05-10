using BubbleBot.Server.Clients;
using BubbleBot.Server.Clients.CollectHDV;
using BubbleBot.Server.Messages;
using System.Threading.Tasks;

namespace BubbleBot.Server.Handlers
{
    public static class CollectHDVHandlers
    {

        public static Task HandleCollectHDVMessage(Client client, CollectHDVMessage message)
    => Task.Run(() =>
    {
        if (!client.LoggedIn)
            return;

        CollectHDV methodeCollect = new CollectHDV();

        methodeCollect.SendItemCollectedInformations(message.ObjectId, message.ObjectName, message.Server, message.PriceLot1, message.PriceLot10, message.PriceLot100, message.AveragePrice);

    });




    }
}
