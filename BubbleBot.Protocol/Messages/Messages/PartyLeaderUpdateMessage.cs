using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PartyLeaderUpdateMessage : AbstractPartyEventMessage
	{

		// Properties
		public uint PartyLeaderId { get; set; }


		// Constructors
		public PartyLeaderUpdateMessage() { }

		public PartyLeaderUpdateMessage(uint partyId = 0, uint partyLeaderId = 0)
		{
			PartyId = partyId;
			PartyLeaderId = partyLeaderId;
		}

	}
}
