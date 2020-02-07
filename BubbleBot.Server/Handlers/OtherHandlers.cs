using System.Threading.Tasks;
using BubbleBot.Server.Clients;
using BubbleBot.Server.Messages;

namespace BubbleBot.Server.Handlers
{
    public static class OtherHandlers
    {

        public static Task HandleFilesHashesRequestMessage(Client client, FilesHashesRequestMessage message)
            => Task.Run(() =>
            {
                client.SendMessage(new FilesHashesMessage(Constants.FilesHashes));
            });


        public static Task HandlePongMessage(Client client, PongMessage message)
            => Task.Run(() =>
            {
                client.StopPingTimeoutTimer();
            });

    }
}
