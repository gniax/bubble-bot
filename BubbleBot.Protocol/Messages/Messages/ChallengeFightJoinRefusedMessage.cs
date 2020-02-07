using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ChallengeFightJoinRefusedMessage : Message
	{

		// Properties
		public uint PlayerId { get; set; }
		public int Reason { get; set; }


		// Constructors
		public ChallengeFightJoinRefusedMessage() { }

		public ChallengeFightJoinRefusedMessage(uint playerId = 0, int reason = 0)
		{
			PlayerId = playerId;
			Reason = reason;
		}

	}
}
