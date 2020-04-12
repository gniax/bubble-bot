namespace BubbleBot.Protocol.Messages
{
    public class PartyInvitationArenaRequestMessage : PartyInvitationRequestMessage
    {

        // Constructors
        public PartyInvitationArenaRequestMessage() { }

        public PartyInvitationArenaRequestMessage(string name = "")
        {
            Name = name;
        }

    }
}
