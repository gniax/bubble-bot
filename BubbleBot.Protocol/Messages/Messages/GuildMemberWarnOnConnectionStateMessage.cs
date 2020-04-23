namespace BubbleBot.Protocol.Messages
{
    public class GuildMemberWarnOnConnectionStateMessage : Message
    {

        // Properties
        public bool Enable { get; set; }


        // Constructors
        public GuildMemberWarnOnConnectionStateMessage() { }

        public GuildMemberWarnOnConnectionStateMessage(bool enable = false)
        {
            Enable = enable;
        }

    }
}
