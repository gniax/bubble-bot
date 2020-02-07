using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildFactsRequestMessage : Message
	{

		// Properties
		public uint GuildId { get; set; }


		// Constructors
		public GuildFactsRequestMessage() { }

		public GuildFactsRequestMessage(uint guildId = 0)
		{
			GuildId = guildId;
		}

	}
}
