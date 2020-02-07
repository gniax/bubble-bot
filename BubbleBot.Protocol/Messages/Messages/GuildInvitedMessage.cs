using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildInvitedMessage : Message
	{

		// Properties
		public uint RecruterId { get; set; }
		public string RecruterName { get; set; }
		public BasicGuildInformations GuildInfo { get; set; }


		// Constructors
		public GuildInvitedMessage() { }

		public GuildInvitedMessage(uint recruterId = 0, string recruterName = "", BasicGuildInformations guildInfo = null)
		{
			RecruterId = recruterId;
			RecruterName = recruterName;
			GuildInfo = guildInfo;
		}

	}
}
