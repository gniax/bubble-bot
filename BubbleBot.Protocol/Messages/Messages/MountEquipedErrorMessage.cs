namespace BubbleBot.Protocol.Messages
{
    public class MountEquipedErrorMessage : Message
    {

        // Properties
        public uint ErrorType { get; set; }


        // Constructors
        public MountEquipedErrorMessage() { }

        public MountEquipedErrorMessage(uint errorType = 0)
        {
            ErrorType = errorType;
        }

    }
}
