namespace BubbleBot.Protocol.Messages
{
    public class PrismInfoInValidMessage : Message
    {

        // Properties
        public uint Reason { get; set; }


        // Constructors
        public PrismInfoInValidMessage() { }

        public PrismInfoInValidMessage(uint reason = 0)
        {
            Reason = reason;
        }

    }
}
