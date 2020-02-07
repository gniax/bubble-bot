using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PartyStopFollowRequestMessage : AbstractPartyMessage
	{

		// Constructors
		public PartyStopFollowRequestMessage() { }

		public PartyStopFollowRequestMessage(uint partyId = 0)
		{
			PartyId = partyId;
		}

	}
}
