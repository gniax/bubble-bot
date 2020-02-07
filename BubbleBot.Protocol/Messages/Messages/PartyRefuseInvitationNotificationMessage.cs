using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PartyRefuseInvitationNotificationMessage : AbstractPartyEventMessage
	{

		// Properties
		public uint GuestId { get; set; }


		// Constructors
		public PartyRefuseInvitationNotificationMessage() { }

		public PartyRefuseInvitationNotificationMessage(uint partyId = 0, uint guestId = 0)
		{
			PartyId = partyId;
			GuestId = guestId;
		}

	}
}
