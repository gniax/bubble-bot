using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeObjectModifiedMessage : ExchangeObjectMessage
    {

        // Properties
        public ObjectItem @Object { get; set; }


        // Constructors
        public ExchangeObjectModifiedMessage() { }

        public ExchangeObjectModifiedMessage(bool remote = false, ObjectItem @object = null)
        {
            Remote = remote;
            @Object = @object;
        }

    }
}
