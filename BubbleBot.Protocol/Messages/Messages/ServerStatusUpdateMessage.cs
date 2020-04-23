using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class ServerStatusUpdateMessage : Message
    {

        // Properties
        public GameServerInformations Server { get; set; }


        // Constructors
        public ServerStatusUpdateMessage() { }

        public ServerStatusUpdateMessage(GameServerInformations server = null)
        {
            Server = server;
        }

    }
}
