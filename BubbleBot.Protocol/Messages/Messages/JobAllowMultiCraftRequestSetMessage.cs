namespace BubbleBot.Protocol.Messages
{
    public class JobAllowMultiCraftRequestSetMessage : Message
    {

        // Properties
        public bool Enabled { get; set; }


        // Constructors
        public JobAllowMultiCraftRequestSetMessage() { }

        public JobAllowMultiCraftRequestSetMessage(bool enabled = false)
        {
            Enabled = enabled;
        }

    }
}
