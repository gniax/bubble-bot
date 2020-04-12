namespace BubbleBot.Protocol.Messages
{
    public class ExchangeRequestMessage : Message
    {

        // Properties
        public int ExchangeType { get; set; }


        // Constructors
        public ExchangeRequestMessage() { }

        public ExchangeRequestMessage(int exchangeType = 0)
        {
            ExchangeType = exchangeType;
        }

    }
}
