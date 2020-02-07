using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildInvitationStateRecrutedMessage : Message
	{

		// Properties
		public uint InvitationState { get; set; }


		// Constructors
		public GuildInvitationStateRecrutedMessage() { }

		public GuildInvitationStateRecrutedMessage(uint invitationState = 0)
		{
			InvitationState = invitationState;
		}

	}
}
