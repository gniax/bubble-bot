using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ServersListMessage : Message
    {

        // Properties
        public List<GameServerInformations> Servers { get; set; }


        // Constructors
        public ServersListMessage() { }

        public ServersListMessage(List<GameServerInformations> servers = null)
        {
            Servers = servers;
        }

    }
}
