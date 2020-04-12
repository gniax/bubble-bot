using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeShopStockMovementUpdatedMessage : Message
    {

        // Properties
        public ObjectItemToSell ObjectInfo { get; set; }


        // Constructors
        public ExchangeShopStockMovementUpdatedMessage() { }

        public ExchangeShopStockMovementUpdatedMessage(ObjectItemToSell objectInfo = null)
        {
            ObjectInfo = objectInfo;
        }

    }
}
