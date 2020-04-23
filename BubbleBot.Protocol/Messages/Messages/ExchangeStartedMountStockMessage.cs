using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeStartedMountStockMessage : Message
    {

        // Properties
        public List<ObjectItem> ObjectsInfos { get; set; }


        // Constructors
        public ExchangeStartedMountStockMessage() { }

        public ExchangeStartedMountStockMessage(List<ObjectItem> objectsInfos = null)
        {
            ObjectsInfos = objectsInfos;
        }

    }
}
