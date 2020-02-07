using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AchievementDetailedListRequestMessage : Message
	{

		// Properties
		public uint CategoryId { get; set; }


		// Constructors
		public AchievementDetailedListRequestMessage() { }

		public AchievementDetailedListRequestMessage(uint categoryId = 0)
		{
			CategoryId = categoryId;
		}

	}
}
