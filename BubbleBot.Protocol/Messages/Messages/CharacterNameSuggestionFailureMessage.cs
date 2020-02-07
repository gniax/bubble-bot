using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class CharacterNameSuggestionFailureMessage : Message
	{

		// Properties
		public uint Reason { get; set; }


		// Constructors
		public CharacterNameSuggestionFailureMessage() { }

		public CharacterNameSuggestionFailureMessage(uint reason = 1)
		{
			Reason = reason;
		}

	}
}
