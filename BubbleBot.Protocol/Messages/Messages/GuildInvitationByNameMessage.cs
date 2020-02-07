using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildInvitationByNameMessage : Message
	{

		// Properties
		public string Name { get; set; }


		// Constructors
		public GuildInvitationByNameMessage() { }

		public GuildInvitationByNameMessage(string name = "")
		{
			Name = name;
		}

	}
}
