namespace BubbleBot.Protocol.Messages
{
    public class LockableCodeResultMessage : Message
    {

        // Properties
        public uint Result { get; set; }


        // Constructors
        public LockableCodeResultMessage() { }

        public LockableCodeResultMessage(uint result = 0)
        {
            Result = result;
        }

    }
}
