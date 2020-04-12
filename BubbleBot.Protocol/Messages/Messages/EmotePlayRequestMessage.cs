namespace BubbleBot.Protocol.Messages
{
    public class EmotePlayRequestMessage : Message
    {

        // Properties
        public uint EmoteId { get; set; }


        // Constructors
        public EmotePlayRequestMessage() { }

        public EmotePlayRequestMessage(uint emoteId = 0)
        {
            EmoteId = emoteId;
        }

    }
}
