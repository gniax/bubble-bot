using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PartyLeaveMessage : AbstractPartyMessage
	{

		// Constructors
		public PartyLeaveMessage() { }

		public PartyLeaveMessage(uint partyId = 0)
		{
			PartyId = partyId;
		}

	}
}
