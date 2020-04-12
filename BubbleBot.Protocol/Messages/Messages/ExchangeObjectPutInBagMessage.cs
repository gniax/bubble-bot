using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeObjectPutInBagMessage : ExchangeObjectMessage
    {

        // Properties
        public ObjectItem @Object { get; set; }


        // Constructors
        public ExchangeObjectPutInBagMessage() { }

        public ExchangeObjectPutInBagMessage(bool remote = false, ObjectItem @object = null)
        {
            Remote = remote;
            @Object = @object;
        }

    }
}
