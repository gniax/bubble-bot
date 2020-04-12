namespace BubbleBot.Protocol.Messages
{
    public class GuildUIOpenedMessage : Message
    {

        // Properties
        public uint Type { get; set; }


        // Constructors
        public GuildUIOpenedMessage() { }

        public GuildUIOpenedMessage(uint type = 0)
        {
            Type = type;
        }

    }
}
