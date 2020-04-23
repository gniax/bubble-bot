using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GuildListMessage : Message
    {

        // Properties
        public List<GuildInformations> Guilds { get; set; }


        // Constructors
        public GuildListMessage() { }

        public GuildListMessage(List<GuildInformations> guilds = null)
        {
            Guilds = guilds;
        }

    }
}
