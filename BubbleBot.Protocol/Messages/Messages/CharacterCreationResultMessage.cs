using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class CharacterCreationResultMessage : Message
	{

		// Properties
		public uint Result { get; set; }


		// Constructors
		public CharacterCreationResultMessage() { }

		public CharacterCreationResultMessage(uint result = 1)
		{
			Result = result;
		}

	}
}
