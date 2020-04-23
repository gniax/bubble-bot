namespace BubbleBot.Protocol.Messages
{
    public class DebugInClientMessage : Message
    {

        // Properties
        public uint Level { get; set; }
        public string Message { get; set; }


        // Constructors
        public DebugInClientMessage() { }

        public DebugInClientMessage(uint level = 0, string message = "")
        {
            Level = level;
            Message = message;
        }

    }
}
