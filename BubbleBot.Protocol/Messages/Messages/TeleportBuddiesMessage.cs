using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class TeleportBuddiesMessage : Message
	{

		// Properties
		public uint DungeonId { get; set; }


		// Constructors
		public TeleportBuddiesMessage() { }

		public TeleportBuddiesMessage(uint dungeonId = 0)
		{
			DungeonId = dungeonId;
		}

	}
}
