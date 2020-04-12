namespace BubbleBot.Protocol.Messages
{
    public class InventoryPresetDeleteMessage : Message
    {

        // Properties
        public uint PresetId { get; set; }


        // Constructors
        public InventoryPresetDeleteMessage() { }

        public InventoryPresetDeleteMessage(uint presetId = 0)
        {
            PresetId = presetId;
        }

    }
}
