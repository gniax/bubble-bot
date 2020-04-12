using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class DungeonPartyFinderRegisterSuccessMessage : Message
    {

        // Properties
        public List<uint> DungeonIds { get; set; }


        // Constructors
        public DungeonPartyFinderRegisterSuccessMessage() { }

        public DungeonPartyFinderRegisterSuccessMessage(List<uint> dungeonIds = null)
        {
            DungeonIds = dungeonIds;
        }

    }
}
