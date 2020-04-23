namespace BubbleBot.Protocol.Messages
{
    public class FriendSetWarnOnLevelGainMessage : Message
    {

        // Properties
        public bool Enable { get; set; }


        // Constructors
        public FriendSetWarnOnLevelGainMessage() { }

        public FriendSetWarnOnLevelGainMessage(bool enable = false)
        {
            Enable = enable;
        }

    }
}
