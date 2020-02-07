using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameRolePlayShowChallengeMessage : Message
	{

		// Properties
		public FightCommonInformations CommonsInfos { get; set; }


		// Constructors
		public GameRolePlayShowChallengeMessage() { }

		public GameRolePlayShowChallengeMessage(FightCommonInformations commonsInfos = null)
		{
			CommonsInfos = commonsInfos;
		}

	}
}
