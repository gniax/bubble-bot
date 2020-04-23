using BubbleBot.Server.Clients;
using BubbleBot.Server.Messages;
using BubbleBot.Server.Utility.Extensions;
using System.Threading.Tasks;

namespace BubbleBot.Server.Handlers
{
    public static class OtherHandlers
    {

        public static Task HandleMissingEntityMessage(Client client, MissingEntityMessage message)
            => Task.Run(() =>
            {

                string itemName = message.Name;
                itemName = itemName.Replace("'", "733");
                itemName = itemName.Replace("%", "734");
                itemName = itemName.Replace("*", "735");
                itemName = itemName.Replace(":", "736");
                itemName = itemName.Replace("/", "737");
                itemName = itemName.Replace("-", "738");
                itemName = itemName.Replace(" ", "_");

                string[] data = new string[] { itemName, message.GID.ToString() };

                if (!FileWriter.FileContainsWords(data, "MissingEntities.bbot"))
                {
                    string formatedData = itemName + " = " + message.GID.ToString() + ",";
                    FileWriter.EmptyWriteMessage(formatedData, "MissingEntities.bbot");
                }

            });

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
