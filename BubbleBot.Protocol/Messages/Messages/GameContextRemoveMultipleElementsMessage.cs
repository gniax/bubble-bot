using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GameContextRemoveMultipleElementsMessage : Message
    {

        // Properties
        public List<int> Id { get; set; }


        // Constructors
        public GameContextRemoveMultipleElementsMessage() { }

        public GameContextRemoveMultipleElementsMessage(List<int> id = null)
        {
            Id = id;
        }

    }
}
