using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class DungeonEnteredMessage : Message
	{

		// Properties
		public uint DungeonId { get; set; }


		// Constructors
		public DungeonEnteredMessage() { }

		public DungeonEnteredMessage(uint dungeonId = 0)
		{
			DungeonId = dungeonId;
		}

	}
}
