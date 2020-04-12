namespace BubbleBot.Protocol.Messages
{
    public class PauseDialogMessage : Message
    {

        // Properties
        public uint DialogType { get; set; }


        // Constructors
        public PauseDialogMessage() { }

        public PauseDialogMessage(uint dialogType = 0)
        {
            DialogType = dialogType;
        }

    }
}
