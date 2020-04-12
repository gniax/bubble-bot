namespace BubbleBot.Protocol.Messages
{
    public class ExchangeReplayMessage : Message
    {

        // Properties
        public int Count { get; set; }


        // Constructors
        public ExchangeReplayMessage() { }

        public ExchangeReplayMessage(int count = 0)
        {
            Count = count;
        }

    }
}
