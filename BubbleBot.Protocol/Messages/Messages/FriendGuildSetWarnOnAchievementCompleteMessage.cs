namespace BubbleBot.Protocol.Messages
{
    public class FriendGuildSetWarnOnAchievementCompleteMessage : Message
    {

        // Properties
        public bool Enable { get; set; }


        // Constructors
        public FriendGuildSetWarnOnAchievementCompleteMessage() { }

        public FriendGuildSetWarnOnAchievementCompleteMessage(bool enable = false)
        {
            Enable = enable;
        }

    }
}
