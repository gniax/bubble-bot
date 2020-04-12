using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class UpdateMapPlayersAgressableStatusMessage : Message
    {

        // Properties
        public List<uint> PlayerIds { get; set; }
        public List<uint> Enable { get; set; }


        // Constructors
        public UpdateMapPlayersAgressableStatusMessage() { }

        public UpdateMapPlayersAgressableStatusMessage(List<uint> playerIds = null, List<uint> enable = null)
        {
            PlayerIds = playerIds;
            Enable = enable;
        }

    }
}
