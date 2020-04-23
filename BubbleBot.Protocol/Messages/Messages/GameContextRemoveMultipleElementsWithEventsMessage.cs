using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GameContextRemoveMultipleElementsWithEventsMessage : GameContextRemoveMultipleElementsMessage
    {

        // Properties
        public List<uint> ElementEventIds { get; set; }


        // Constructors
        public GameContextRemoveMultipleElementsWithEventsMessage() { }

        public GameContextRemoveMultipleElementsWithEventsMessage(List<int> id = null, List<uint> elementEventIds = null)
        {
            Id = id;
            ElementEventIds = elementEventIds;
        }

    }
}
