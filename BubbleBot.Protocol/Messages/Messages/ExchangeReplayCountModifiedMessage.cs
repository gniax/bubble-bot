namespace BubbleBot.Protocol.Messages
{
    public class ExchangeReplayCountModifiedMessage : Message
    {

        // Properties
        public int Count { get; set; }


        // Constructors
        public ExchangeReplayCountModifiedMessage() { }

        public ExchangeReplayCountModifiedMessage(int count = 0)
        {
            Count = count;
        }

    }
}
