using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class FriendGuildWarnOnAchievementCompleteStateMessage : Message
	{

		// Properties
		public bool Enable { get; set; }


		// Constructors
		public FriendGuildWarnOnAchievementCompleteStateMessage() { }

		public FriendGuildWarnOnAchievementCompleteStateMessage(bool enable = false)
		{
			Enable = enable;
		}

	}
}
