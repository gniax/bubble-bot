namespace BubbleBot.Protocol.Messages
{
    public class AllianceInvitationMessage : Message
    {

        // Properties
        public uint TargetId { get; set; }


        // Constructors
        public AllianceInvitationMessage() { }

        public AllianceInvitationMessage(uint targetId = 0)
        {
            TargetId = targetId;
        }

    }
}
