using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameFightNewRoundMessage : Message
	{

		// Properties
		public uint RoundNumber { get; set; }


		// Constructors
		public GameFightNewRoundMessage() { }

		public GameFightNewRoundMessage(uint roundNumber = 0)
		{
			RoundNumber = roundNumber;
		}

	}
}
