namespace BubbleBot.Protocol.Messages
{
    public class ExchangeWeightMessage : Message
    {

        // Properties
        public uint CurrentWeight { get; set; }
        public uint MaxWeight { get; set; }


        // Constructors
        public ExchangeWeightMessage() { }

        public ExchangeWeightMessage(uint currentWeight = 0, uint maxWeight = 0)
        {
            CurrentWeight = currentWeight;
            MaxWeight = maxWeight;
        }

    }
}
