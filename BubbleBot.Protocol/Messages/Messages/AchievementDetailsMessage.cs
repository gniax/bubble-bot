using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AchievementDetailsMessage : Message
	{

		// Properties
		public Achievement Achievement { get; set; }


		// Constructors
		public AchievementDetailsMessage() { }

		public AchievementDetailsMessage(Achievement achievement = null)
		{
			Achievement = achievement;
		}

	}
}
