namespace BubbleBot.Protocol.Messages
{
    public class MountDataErrorMessage : Message
    {

        // Properties
        public uint Reason { get; set; }


        // Constructors
        public MountDataErrorMessage() { }

        public MountDataErrorMessage(uint reason = 0)
        {
            Reason = reason;
        }

    }
}
