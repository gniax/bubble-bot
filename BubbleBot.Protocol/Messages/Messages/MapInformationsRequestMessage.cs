using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class MapInformationsRequestMessage : Message
	{

		// Properties
		public uint MapId { get; set; }


		// Constructors
		public MapInformationsRequestMessage() { }

		public MapInformationsRequestMessage(uint mapId = 0)
		{
			MapId = mapId;
		}

	}
}
