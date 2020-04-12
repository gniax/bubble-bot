namespace BubbleBot.Protocol.Messages
{
    public class ShortcutBarRemoveRequestMessage : Message
    {

        // Properties
        public uint BarType { get; set; }
        public uint Slot { get; set; }


        // Constructors
        public ShortcutBarRemoveRequestMessage() { }

        public ShortcutBarRemoveRequestMessage(uint barType = 0, uint slot = 0)
        {
            BarType = barType;
            Slot = slot;
        }

    }
}
