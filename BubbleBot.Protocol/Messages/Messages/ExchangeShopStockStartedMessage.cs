using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeShopStockStartedMessage : Message
    {

        // Properties
        public List<ObjectItemToSell> ObjectsInfos { get; set; }


        // Constructors
        public ExchangeShopStockStartedMessage() { }

        public ExchangeShopStockStartedMessage(List<ObjectItemToSell> objectsInfos = null)
        {
            ObjectsInfos = objectsInfos;
        }

    }
}
