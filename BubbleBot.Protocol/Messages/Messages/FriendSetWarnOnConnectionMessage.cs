using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class FriendSetWarnOnConnectionMessage : Message
	{

		// Properties
		public bool Enable { get; set; }


		// Constructors
		public FriendSetWarnOnConnectionMessage() { }

		public FriendSetWarnOnConnectionMessage(bool enable = false)
		{
			Enable = enable;
		}

	}
}
