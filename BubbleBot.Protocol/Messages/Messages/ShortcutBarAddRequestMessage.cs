using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class ShortcutBarAddRequestMessage : Message
    {

        // Properties
        public uint BarType { get; set; }
        public Shortcut Shortcut { get; set; }


        // Constructors
        public ShortcutBarAddRequestMessage() { }

        public ShortcutBarAddRequestMessage(uint barType = 0, Shortcut shortcut = null)
        {
            BarType = barType;
            Shortcut = shortcut;
        }

    }
}
