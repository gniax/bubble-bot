using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class FriendWarnOnLevelGainStateMessage : Message
	{

		// Properties
		public bool Enable { get; set; }


		// Constructors
		public FriendWarnOnLevelGainStateMessage() { }

		public FriendWarnOnLevelGainStateMessage(bool enable = false)
		{
			Enable = enable;
		}

	}
}
