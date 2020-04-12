namespace BubbleBot.Protocol.Messages
{
    public class FriendJoinRequestMessage : Message
    {

        // Properties
        public string Name { get; set; }


        // Constructors
        public FriendJoinRequestMessage() { }

        public FriendJoinRequestMessage(string name = "")
        {
            Name = name;
        }

    }
}
