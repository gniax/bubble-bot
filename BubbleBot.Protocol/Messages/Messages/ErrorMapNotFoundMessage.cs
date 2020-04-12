namespace BubbleBot.Protocol.Messages
{
    public class ErrorMapNotFoundMessage : Message
    {

        // Properties
        public uint MapId { get; set; }


        // Constructors
        public ErrorMapNotFoundMessage() { }

        public ErrorMapNotFoundMessage(uint mapId = 0)
        {
            MapId = mapId;
        }

    }
}
