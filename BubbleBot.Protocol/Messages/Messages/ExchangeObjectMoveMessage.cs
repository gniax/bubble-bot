namespace BubbleBot.Protocol.Messages
{
    public class ExchangeObjectMoveMessage : Message
    {

        // Properties
        public uint ObjectUID { get; set; }
        public int Quantity { get; set; }
        public uint Price { get; set; }


        // Constructors
        public ExchangeObjectMoveMessage() { }

        public ExchangeObjectMoveMessage(uint objectUID = 0, int quantity = 0)
        {
            ObjectUID = objectUID;
            Quantity = quantity;
        }

    }
}
