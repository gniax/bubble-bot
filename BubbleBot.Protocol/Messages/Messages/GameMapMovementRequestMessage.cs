using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GameMapMovementRequestMessage : Message
    {

        // Properties
        public List<int> KeyMovements { get; set; }
        public uint MapId { get; set; }


        // Constructors
        public GameMapMovementRequestMessage() { }

        public GameMapMovementRequestMessage(uint mapId = 0, List<int> keyMovements = null)
        {
            MapId = mapId;
            KeyMovements = keyMovements;
        }

    }
}
