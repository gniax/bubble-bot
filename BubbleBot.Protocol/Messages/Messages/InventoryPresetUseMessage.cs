using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class InventoryPresetUseMessage : Message
	{

		// Properties
		public uint PresetId { get; set; }


		// Constructors
		public InventoryPresetUseMessage() { }

		public InventoryPresetUseMessage(uint presetId = 0)
		{
			PresetId = presetId;
		}

	}
}
