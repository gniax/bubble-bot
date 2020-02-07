using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PartyFollowStatusUpdateMessage : AbstractPartyMessage
	{

		// Properties
		public bool Success { get; set; }
		public uint FollowedId { get; set; }


		// Constructors
		public PartyFollowStatusUpdateMessage() { }

		public PartyFollowStatusUpdateMessage(uint partyId = 0, bool success = false, uint followedId = 0)
		{
			PartyId = partyId;
			Success = success;
			FollowedId = followedId;
		}

	}
}
