using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameActionFightCastRequestMessage : Message
	{

		// Properties
		public uint SpellId { get; set; }
		public int CellId { get; set; }


		// Constructors
		public GameActionFightCastRequestMessage() { }

		public GameActionFightCastRequestMessage(uint spellId = 0, int cellId = 0)
		{
			SpellId = spellId;
			CellId = cellId;
		}

	}
}
