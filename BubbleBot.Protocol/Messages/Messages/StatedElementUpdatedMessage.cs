using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class StatedElementUpdatedMessage : Message
    {

        // Properties
        public StatedElement StatedElement { get; set; }


        // Constructors
        public StatedElementUpdatedMessage() { }

        public StatedElementUpdatedMessage(StatedElement statedElement = null)
        {
            StatedElement = statedElement;
        }

    }
}
