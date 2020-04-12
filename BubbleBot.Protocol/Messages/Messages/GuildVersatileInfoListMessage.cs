using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GuildVersatileInfoListMessage : Message
    {

        // Properties
        public List<GuildVersatileInformations> Guilds { get; set; }


        // Constructors
        public GuildVersatileInfoListMessage() { }

        public GuildVersatileInfoListMessage(List<GuildVersatileInformations> guilds = null)
        {
            Guilds = guilds;
        }

    }
}
