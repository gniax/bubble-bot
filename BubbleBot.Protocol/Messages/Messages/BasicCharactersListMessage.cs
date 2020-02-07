using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class BasicCharactersListMessage : Message
	{

		// Properties
		public List<CharacterBaseInformations> Characters { get; set; }


		// Constructors
		public BasicCharactersListMessage() { }

		public BasicCharactersListMessage(List<CharacterBaseInformations> characters = null)
		{
			Characters = characters;
		}

	}
}
