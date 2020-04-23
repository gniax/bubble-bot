namespace BubbleBot.Protocol.Messages
{
    public class ExchangeObjectMovePricedMessage : ExchangeObjectMoveMessage
    {

        // Properties
        public new int Price { get; set; }

        // Constructors
        public ExchangeObjectMovePricedMessage() { }

        public ExchangeObjectMovePricedMessage(uint objectUID = 0, int quantity = 0, int price = 0)
        {
            ObjectUID = objectUID;
            Quantity = quantity;
            Price = price;
        }

    }
}
