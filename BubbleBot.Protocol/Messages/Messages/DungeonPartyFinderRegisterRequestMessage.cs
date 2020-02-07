using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class DungeonPartyFinderRegisterRequestMessage : Message
	{

		// Properties
		public List<uint> DungeonIds { get; set; }


		// Constructors
		public DungeonPartyFinderRegisterRequestMessage() { }

		public DungeonPartyFinderRegisterRequestMessage(List<uint> dungeonIds = null)
		{
			DungeonIds = dungeonIds;
		}

	}
}
