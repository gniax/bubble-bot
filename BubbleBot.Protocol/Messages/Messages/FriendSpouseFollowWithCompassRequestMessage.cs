using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class FriendSpouseFollowWithCompassRequestMessage : Message
	{

		// Properties
		public bool Enable { get; set; }


		// Constructors
		public FriendSpouseFollowWithCompassRequestMessage() { }

		public FriendSpouseFollowWithCompassRequestMessage(bool enable = false)
		{
			Enable = enable;
		}

	}
}
