using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PartyKickRequestMessage : AbstractPartyMessage
	{

		// Properties
		public uint PlayerId { get; set; }


		// Constructors
		public PartyKickRequestMessage() { }

		public PartyKickRequestMessage(uint partyId = 0, uint playerId = 0)
		{
			PartyId = partyId;
			PlayerId = playerId;
		}

	}
}
