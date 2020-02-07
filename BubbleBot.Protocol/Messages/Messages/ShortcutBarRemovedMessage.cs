using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ShortcutBarRemovedMessage : Message
	{

		// Properties
		public uint BarType { get; set; }
		public uint Slot { get; set; }


		// Constructors
		public ShortcutBarRemovedMessage() { }

		public ShortcutBarRemovedMessage(uint barType = 0, uint slot = 0)
		{
			BarType = barType;
			Slot = slot;
		}

	}
}
