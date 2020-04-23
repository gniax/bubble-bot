using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class DungeonPartyFinderAvailableDungeonsMessage : Message
    {

        // Properties
        public List<uint> DungeonIds { get; set; }


        // Constructors
        public DungeonPartyFinderAvailableDungeonsMessage() { }

        public DungeonPartyFinderAvailableDungeonsMessage(List<uint> dungeonIds = null)
        {
            DungeonIds = dungeonIds;
        }

    }
}
