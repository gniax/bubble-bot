using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeStartedBidSellerMessage : Message
    {

        // Properties
        public List<ObjectItemToSellInBid> ObjectsInfos { get; set; }
        public SellerBuyerDescriptor SellerDescriptor { get; set; }


        // Constructors
        public ExchangeStartedBidSellerMessage() { }

        public ExchangeStartedBidSellerMessage(SellerBuyerDescriptor sellerDescriptor = null, List<ObjectItemToSellInBid> objectsInfos = null)
        {
            SellerDescriptor = sellerDescriptor;
            ObjectsInfos = objectsInfos;
        }

    }
}
