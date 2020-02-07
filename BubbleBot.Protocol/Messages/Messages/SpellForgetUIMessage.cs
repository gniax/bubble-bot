using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class SpellForgetUIMessage : Message
	{

		// Properties
		public bool Open { get; set; }


		// Constructors
		public SpellForgetUIMessage() { }

		public SpellForgetUIMessage(bool open = false)
		{
			Open = open;
		}

	}
}
