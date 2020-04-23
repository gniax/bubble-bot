using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeShopStockMultiMovementUpdatedMessage : Message
    {

        // Properties
        public List<ObjectItemToSell> ObjectInfoList { get; set; }


        // Constructors
        public ExchangeShopStockMultiMovementUpdatedMessage() { }

        public ExchangeShopStockMultiMovementUpdatedMessage(List<ObjectItemToSell> objectInfoList = null)
        {
            ObjectInfoList = objectInfoList;
        }

    }
}
