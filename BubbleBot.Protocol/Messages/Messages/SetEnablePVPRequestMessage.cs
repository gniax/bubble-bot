namespace BubbleBot.Protocol.Messages
{
    public class SetEnablePVPRequestMessage : Message
    {

        // Properties
        public bool Enable { get; set; }


        // Constructors
        public SetEnablePVPRequestMessage() { }

        public SetEnablePVPRequestMessage(bool enable = false)
        {
            Enable = enable;
        }

    }
}
