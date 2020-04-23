namespace BubbleBot.Protocol.Messages
{
    public class AllianceInvitationAnswerMessage : Message
    {

        // Properties
        public bool Accept { get; set; }


        // Constructors
        public AllianceInvitationAnswerMessage() { }

        public AllianceInvitationAnswerMessage(bool accept = false)
        {
            Accept = accept;
        }

    }
}
