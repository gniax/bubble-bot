using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ValidateSpellForgetMessage : Message
	{

		// Properties
		public uint SpellId { get; set; }


		// Constructors
		public ValidateSpellForgetMessage() { }

		public ValidateSpellForgetMessage(uint spellId = 0)
		{
			SpellId = spellId;
		}

	}
}
