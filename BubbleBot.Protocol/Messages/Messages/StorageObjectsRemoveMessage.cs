using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class StorageObjectsRemoveMessage : Message
    {

        // Properties
        public List<uint> ObjectUIDList { get; set; }


        // Constructors
        public StorageObjectsRemoveMessage() { }

        public StorageObjectsRemoveMessage(List<uint> objectUIDList = null)
        {
            ObjectUIDList = objectUIDList;
        }

    }
}
