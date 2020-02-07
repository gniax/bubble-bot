using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildModificationNameValidMessage : Message
	{

		// Properties
		public string GuildName { get; set; }


		// Constructors
		public GuildModificationNameValidMessage() { }

		public GuildModificationNameValidMessage(string guildName = "")
		{
			GuildName = guildName;
		}

	}
}
