using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ShortcutBarContentMessage : Message
	{

		// Properties
		public List<Shortcut> Shortcuts { get; set; }
		public uint BarType { get; set; }


		// Constructors
		public ShortcutBarContentMessage() { }

		public ShortcutBarContentMessage(uint barType = 0, List<Shortcut> shortcuts = null)
		{
			BarType = barType;
			Shortcuts = shortcuts;
		}

	}
}
