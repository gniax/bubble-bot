using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AchievementRewardRequestMessage : Message
	{

		// Properties
		public int AchievementId { get; set; }


		// Constructors
		public AchievementRewardRequestMessage() { }

		public AchievementRewardRequestMessage(int achievementId = 0)
		{
			AchievementId = achievementId;
		}

	}
}
