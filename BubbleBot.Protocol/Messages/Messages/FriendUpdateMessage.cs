using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class FriendUpdateMessage : Message
	{

		// Properties
		public FriendInformations FriendUpdated { get; set; }


		// Constructors
		public FriendUpdateMessage() { }

		public FriendUpdateMessage(FriendInformations friendUpdated = null)
		{
			FriendUpdated = friendUpdated;
		}

	}
}
