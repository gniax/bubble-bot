using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class InventoryPresetItemUpdateMessage : Message
    {

        // Properties
        public uint PresetId { get; set; }
        public PresetItem PresetItem { get; set; }


        // Constructors
        public InventoryPresetItemUpdateMessage() { }

        public InventoryPresetItemUpdateMessage(uint presetId = 0, PresetItem presetItem = null)
        {
            PresetId = presetId;
            PresetItem = presetItem;
        }

    }
}
