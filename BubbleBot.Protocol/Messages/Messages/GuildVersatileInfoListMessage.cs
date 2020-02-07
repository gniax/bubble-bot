using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildVersatileInfoListMessage : Message
	{

		// Properties
		public List<GuildVersatileInformations> Guilds { get; set; }


		// Constructors
		public GuildVersatileInfoListMessage() { }

		public GuildVersatileInfoListMessage(List<GuildVersatileInformations> guilds = null)
		{
			Guilds = guilds;
		}

	}
}
