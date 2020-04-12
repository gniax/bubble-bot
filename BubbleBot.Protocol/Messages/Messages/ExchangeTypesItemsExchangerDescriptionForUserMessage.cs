using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeTypesItemsExchangerDescriptionForUserMessage : Message
    {

        // Properties
        public List<BidExchangerObjectInfo> ItemTypeDescriptions { get; set; }


        // Constructors
        public ExchangeTypesItemsExchangerDescriptionForUserMessage() { }

        public ExchangeTypesItemsExchangerDescriptionForUserMessage(List<BidExchangerObjectInfo> itemTypeDescriptions = null)
        {
            ItemTypeDescriptions = itemTypeDescriptions;
        }

    }
}
