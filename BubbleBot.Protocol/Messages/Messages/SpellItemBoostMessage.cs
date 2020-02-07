using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class SpellItemBoostMessage : Message
	{

		// Properties
		public uint StatId { get; set; }
		public uint SpellId { get; set; }
		public int Value { get; set; }


		// Constructors
		public SpellItemBoostMessage() { }

		public SpellItemBoostMessage(uint statId = 0, uint spellId = 0, int value = 0)
		{
			StatId = statId;
			SpellId = spellId;
			Value = value;
		}

	}
}
