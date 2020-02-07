using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildListMessage : Message
	{

		// Properties
		public List<GuildInformations> Guilds { get; set; }


		// Constructors
		public GuildListMessage() { }

		public GuildListMessage(List<GuildInformations> guilds = null)
		{
			Guilds = guilds;
		}

	}
}
