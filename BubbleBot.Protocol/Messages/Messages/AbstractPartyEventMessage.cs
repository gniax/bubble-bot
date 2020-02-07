using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AbstractPartyEventMessage : AbstractPartyMessage
	{

		// Constructors
		public AbstractPartyEventMessage() { }

		public AbstractPartyEventMessage(uint partyId = 0)
		{
			PartyId = partyId;
		}

	}
}
