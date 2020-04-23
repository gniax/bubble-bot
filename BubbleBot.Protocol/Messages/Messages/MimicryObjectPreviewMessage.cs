using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class MimicryObjectPreviewMessage : Message
    {

        // Properties
        public ObjectItem Result { get; set; }


        // Constructors
        public MimicryObjectPreviewMessage() { }

        public MimicryObjectPreviewMessage(ObjectItem result = null)
        {
            Result = result;
        }

    }
}
