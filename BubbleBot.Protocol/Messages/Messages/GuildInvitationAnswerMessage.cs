namespace BubbleBot.Protocol.Messages
{
    public class GuildInvitationAnswerMessage : Message
    {

        // Properties
        public bool Accept { get; set; }


        // Constructors
        public GuildInvitationAnswerMessage() { }

        public GuildInvitationAnswerMessage(bool accept = false)
        {
            Accept = accept;
        }

    }
}
