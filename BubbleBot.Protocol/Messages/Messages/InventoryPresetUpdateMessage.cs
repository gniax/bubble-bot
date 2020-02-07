using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class InventoryPresetUpdateMessage : Message
	{

		// Properties
		public Preset Preset { get; set; }


		// Constructors
		public InventoryPresetUpdateMessage() { }

		public InventoryPresetUpdateMessage(Preset preset = null)
		{
			Preset = preset;
		}

	}
}
