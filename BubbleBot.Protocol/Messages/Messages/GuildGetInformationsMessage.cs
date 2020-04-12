namespace BubbleBot.Protocol.Messages
{
    public class GuildGetInformationsMessage : Message
    {

        // Properties
        public uint InfoType { get; set; }


        // Constructors
        public GuildGetInformationsMessage() { }

        public GuildGetInformationsMessage(uint infoType = 0)
        {
            InfoType = infoType;
        }

    }
}
