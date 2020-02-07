using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildKickRequestMessage : Message
	{

		// Properties
		public uint KickedId { get; set; }


		// Constructors
		public GuildKickRequestMessage() { }

		public GuildKickRequestMessage(uint kickedId = 0)
		{
			KickedId = kickedId;
		}

	}
}
