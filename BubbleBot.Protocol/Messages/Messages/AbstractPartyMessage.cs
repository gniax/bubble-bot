using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AbstractPartyMessage : Message
	{

		// Properties
		public uint PartyId { get; set; }


		// Constructors
		public AbstractPartyMessage() { }

		public AbstractPartyMessage(uint partyId = 0)
		{
			PartyId = partyId;
		}

	}
}
