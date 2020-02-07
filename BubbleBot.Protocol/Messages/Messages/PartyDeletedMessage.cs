using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PartyDeletedMessage : AbstractPartyMessage
	{

		// Constructors
		public PartyDeletedMessage() { }

		public PartyDeletedMessage(uint partyId = 0)
		{
			PartyId = partyId;
		}

	}
}
