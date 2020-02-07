using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class InventoryPresetItemUpdateErrorMessage : Message
	{

		// Properties
		public uint Code { get; set; }


		// Constructors
		public InventoryPresetItemUpdateErrorMessage() { }

		public InventoryPresetItemUpdateErrorMessage(uint code = 1)
		{
			Code = code;
		}

	}
}
