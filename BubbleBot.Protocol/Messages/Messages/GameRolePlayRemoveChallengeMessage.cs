using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameRolePlayRemoveChallengeMessage : Message
	{

		// Properties
		public int FightId { get; set; }


		// Constructors
		public GameRolePlayRemoveChallengeMessage() { }

		public GameRolePlayRemoveChallengeMessage(int fightId = 0)
		{
			FightId = fightId;
		}

	}
}
