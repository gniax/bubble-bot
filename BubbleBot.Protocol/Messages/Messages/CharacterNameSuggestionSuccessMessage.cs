using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class CharacterNameSuggestionSuccessMessage : Message
	{

		// Properties
		public string Suggestion { get; set; }


		// Constructors
		public CharacterNameSuggestionSuccessMessage() { }

		public CharacterNameSuggestionSuccessMessage(string suggestion = "")
		{
			Suggestion = suggestion;
		}

	}
}
