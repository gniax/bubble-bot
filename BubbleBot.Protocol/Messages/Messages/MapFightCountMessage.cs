using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class MapFightCountMessage : Message
	{

		// Properties
		public uint FightCount { get; set; }


		// Constructors
		public MapFightCountMessage() { }

		public MapFightCountMessage(uint fightCount = 0)
		{
			FightCount = fightCount;
		}

	}
}
