namespace BubbleBot.Protocol.Messages
{
    public class InventoryPresetDeleteResultMessage : Message
    {

        // Properties
        public uint PresetId { get; set; }
        public uint Code { get; set; }


        // Constructors
        public InventoryPresetDeleteResultMessage() { }

        public InventoryPresetDeleteResultMessage(uint presetId = 0, uint code = 2)
        {
            PresetId = presetId;
            Code = code;
        }

    }
}
