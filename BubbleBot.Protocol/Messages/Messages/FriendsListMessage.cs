using BubbleBot.Protocol.Types;
using System.Collections.Generic;

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
