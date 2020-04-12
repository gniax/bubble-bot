using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeObjectAddedMessage : ExchangeObjectMessage
    {

        // Properties
        public ObjectItem @Object { get; set; }


        // Constructors
        public ExchangeObjectAddedMessage() { }

        public ExchangeObjectAddedMessage(bool remote = false, ObjectItem @object = null)
        {
            Remote = remote;
            @Object = @object;
        }

    }
}
