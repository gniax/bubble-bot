using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class FriendsListMessage : Message
	{

		// Properties
		public List<FriendInformations> FriendsList { get; set; }


		// Constructors
		public FriendsListMessage() { }

		public FriendsListMessage(List<FriendInformations> friendsList = null)
		{
			FriendsList = friendsList;
		}

	}
}
