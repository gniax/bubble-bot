namespace BubbleBot.Protocol.Messages
{
    public class TitleSelectErrorMessage : Message
    {

        // Properties
        public uint Reason { get; set; }


        // Constructors
        public TitleSelectErrorMessage() { }

        public TitleSelectErrorMessage(uint reason = 0)
        {
            Reason = reason;
        }

    }
}
