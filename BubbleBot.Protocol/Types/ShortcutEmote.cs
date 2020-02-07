using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class ShortcutEmote : Shortcut
	{

		// Properties
		public uint EmoteId { get; set; }


		// Constructors
		public ShortcutEmote() { }

		public ShortcutEmote(uint slot = 0, uint emoteId = 0)
		{
			Slot = slot;
			EmoteId = emoteId;
		}

	}
}
