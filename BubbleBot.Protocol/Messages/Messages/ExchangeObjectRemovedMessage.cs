namespace BubbleBot.Protocol.Messages
{
    public class ExchangeObjectRemovedMessage : ExchangeObjectMessage
    {

        // Properties
        public uint ObjectUID { get; set; }


        // Constructors
        public ExchangeObjectRemovedMessage() { }

        public ExchangeObjectRemovedMessage(bool remote = false, uint objectUID = 0)
        {
            Remote = remote;
            ObjectUID = objectUID;
        }

    }
}
