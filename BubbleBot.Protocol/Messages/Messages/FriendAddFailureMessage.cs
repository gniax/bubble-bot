namespace BubbleBot.Protocol.Messages
{
    public class FriendAddFailureMessage : Message
    {

        // Properties
        public uint Reason { get; set; }


        // Constructors
        public FriendAddFailureMessage() { }

        public FriendAddFailureMessage(uint reason = 0)
        {
            Reason = reason;
        }

    }
}
