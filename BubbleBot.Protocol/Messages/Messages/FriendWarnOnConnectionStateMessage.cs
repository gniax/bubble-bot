namespace BubbleBot.Protocol.Messages
{
    public class FriendWarnOnConnectionStateMessage : Message
    {

        // Properties
        public bool Enable { get; set; }


        // Constructors
        public FriendWarnOnConnectionStateMessage() { }

        public FriendWarnOnConnectionStateMessage(bool enable = false)
        {
            Enable = enable;
        }

    }
}
