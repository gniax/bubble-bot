using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GameDataPaddockObjectListAddMessage : Message
    {

        // Properties
        public List<PaddockItem> PaddockItemDescription { get; set; }


        // Constructors
        public GameDataPaddockObjectListAddMessage() { }

        public GameDataPaddockObjectListAddMessage(List<PaddockItem> paddockItemDescription = null)
        {
            PaddockItemDescription = paddockItemDescription;
        }

    }
}
