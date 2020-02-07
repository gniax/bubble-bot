using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PlayerStatusUpdateMessage : Message
	{

		// Properties
		public uint AccountId { get; set; }
		public uint PlayerId { get; set; }
		public PlayerStatus Status { get; set; }


		// Constructors
		public PlayerStatusUpdateMessage() { }

		public PlayerStatusUpdateMessage(uint accountId = 0, uint playerId = 0, PlayerStatus status = null)
		{
			AccountId = accountId;
			PlayerId = playerId;
			Status = status;
		}

	}
}
