using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class EmotePlayMassiveMessage : EmotePlayAbstractMessage
    {

        // Properties
        public List<int> ActorIds { get; set; }


        // Constructors
        public EmotePlayMassiveMessage() { }

        public EmotePlayMassiveMessage(uint emoteId = 0, double emoteStartTime = 0, List<int> actorIds = null)
        {
            EmoteId = emoteId;
            EmoteStartTime = emoteStartTime;
            ActorIds = actorIds;
        }

    }
}
