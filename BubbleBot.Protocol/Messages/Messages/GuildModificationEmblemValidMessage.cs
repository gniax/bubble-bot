using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class GuildModificationEmblemValidMessage : Message
    {

        // Properties
        public GuildEmblem GuildEmblem { get; set; }


        // Constructors
        public GuildModificationEmblemValidMessage() { }

        public GuildModificationEmblemValidMessage(GuildEmblem guildEmblem = null)
        {
            GuildEmblem = guildEmblem;
        }

    }
}
