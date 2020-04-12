namespace BubbleBot.Protocol.Messages
{
    public class OrnamentSelectRequestMessage : Message
    {

        // Properties
        public uint OrnamentId { get; set; }


        // Constructors
        public OrnamentSelectRequestMessage() { }

        public OrnamentSelectRequestMessage(uint ornamentId = 0)
        {
            OrnamentId = ornamentId;
        }

    }
}
