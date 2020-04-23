using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class DungeonPartyFinderRoomContentMessage : Message
    {

        // Properties
        public List<DungeonPartyFinderPlayer> Players { get; set; }
        public uint DungeonId { get; set; }


        // Constructors
        public DungeonPartyFinderRoomContentMessage() { }

        public DungeonPartyFinderRoomContentMessage(uint dungeonId = 0, List<DungeonPartyFinderPlayer> players = null)
        {
            DungeonId = dungeonId;
            Players = players;
        }

    }
}
