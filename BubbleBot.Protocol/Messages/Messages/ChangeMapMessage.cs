using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ChangeMapMessage : Message
	{

		// Properties
		public uint MapId { get; set; }


		// Constructors
		public ChangeMapMessage() { }

		public ChangeMapMessage(uint mapId = 0)
		{
			MapId = mapId;
		}

	}
}
