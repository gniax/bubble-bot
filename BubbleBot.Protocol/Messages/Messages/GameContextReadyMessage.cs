namespace BubbleBot.Protocol.Messages
{
    public class GameContextReadyMessage : Message
    {

        // Properties
        public uint MapId { get; set; }


        // Constructors
        public GameContextReadyMessage() { }

        public GameContextReadyMessage(uint mapId = 0)
        {
            MapId = mapId;
        }

    }
}
