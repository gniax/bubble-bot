using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class FriendAddedMessage : Message
    {

        // Properties
        public FriendInformations FriendAdded { get; set; }


        // Constructors
        public FriendAddedMessage() { }

        public FriendAddedMessage(FriendInformations friendAdded = null)
        {
            FriendAdded = friendAdded;
        }

    }
}
