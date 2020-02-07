using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AchievementRewardSuccessMessage : Message
	{

		// Properties
		public int AchievementId { get; set; }


		// Constructors
		public AchievementRewardSuccessMessage() { }

		public AchievementRewardSuccessMessage(int achievementId = 0)
		{
			AchievementId = achievementId;
		}

	}
}
