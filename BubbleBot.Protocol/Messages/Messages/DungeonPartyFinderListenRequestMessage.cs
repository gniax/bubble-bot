using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class DungeonPartyFinderListenRequestMessage : Message
	{

		// Properties
		public uint DungeonId { get; set; }


		// Constructors
		public DungeonPartyFinderListenRequestMessage() { }

		public DungeonPartyFinderListenRequestMessage(uint dungeonId = 0)
		{
			DungeonId = dungeonId;
		}

	}
}
