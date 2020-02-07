using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildHouseRemoveMessage : Message
	{

		// Properties
		public uint HouseId { get; set; }


		// Constructors
		public GuildHouseRemoveMessage() { }

		public GuildHouseRemoveMessage(uint houseId = 0)
		{
			HouseId = houseId;
		}

	}
}
