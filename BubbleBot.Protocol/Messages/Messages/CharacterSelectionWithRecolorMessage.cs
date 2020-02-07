using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class CharacterSelectionWithRecolorMessage : CharacterSelectionMessage
	{

		// Properties
		public List<int> IndexedColor { get; set; }


		// Constructors
		public CharacterSelectionWithRecolorMessage() { }

		public CharacterSelectionWithRecolorMessage(int id = 0, List<int> indexedColor = null)
		{
			Id = id;
			IndexedColor = indexedColor;
		}

	}
}
