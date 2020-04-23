using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeBidHouseInListUpdatedMessage : ExchangeBidHouseInListAddedMessage
    {

        // Constructors
        public ExchangeBidHouseInListUpdatedMessage() { }

        public ExchangeBidHouseInListUpdatedMessage(int itemUID = 0, int objGenericId = 0, List<ObjectEffect> effects = null, List<uint> prices = null)
        {
            ItemUID = itemUID;
            ObjGenericId = objGenericId;
            Effects = effects;
            Prices = prices;
        }

    }
}
