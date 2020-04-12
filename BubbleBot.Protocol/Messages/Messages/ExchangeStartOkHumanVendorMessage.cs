using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeStartOkHumanVendorMessage : Message
    {

        // Properties
        public List<ObjectItemToSellInHumanVendorShop> ObjectsInfos { get; set; }
        public uint SellerId { get; set; }


        // Constructors
        public ExchangeStartOkHumanVendorMessage() { }

        public ExchangeStartOkHumanVendorMessage(uint sellerId = 0, List<ObjectItemToSellInHumanVendorShop> objectsInfos = null)
        {
            SellerId = sellerId;
            ObjectsInfos = objectsInfos;
        }

    }
}
