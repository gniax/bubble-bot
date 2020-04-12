namespace BubbleBot.Protocol.Messages
{
    public class ExchangeWaitingResultMessage : Message
    {

        // Properties
        public bool Bwait { get; set; }


        // Constructors
        public ExchangeWaitingResultMessage() { }

        public ExchangeWaitingResultMessage(bool bwait = false)
        {
            Bwait = bwait;
        }

    }
}
