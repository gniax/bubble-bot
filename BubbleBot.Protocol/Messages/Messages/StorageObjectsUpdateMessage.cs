using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class StorageObjectsUpdateMessage : Message
    {

        // Properties
        public List<ObjectItem> ObjectList { get; set; }


        // Constructors
        public StorageObjectsUpdateMessage() { }

        public StorageObjectsUpdateMessage(List<ObjectItem> objectList = null)
        {
            ObjectList = objectList;
        }

    }
}
