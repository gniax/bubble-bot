using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class ObjectAddedMessage : Message
    {

        // Properties
        public ObjectItem @Object { get; set; }


        // Constructors
        public ObjectAddedMessage() { }

        public ObjectAddedMessage(ObjectItem @object = null)
        {
            @Object = @object;
        }

    }
}
