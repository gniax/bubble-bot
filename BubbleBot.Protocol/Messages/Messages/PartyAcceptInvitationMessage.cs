namespace BubbleBot.Protocol.Messages
{
    public class PartyAcceptInvitationMessage : AbstractPartyMessage
    {

        // Constructors
        public PartyAcceptInvitationMessage() { }

        public PartyAcceptInvitationMessage(uint partyId = 0)
        {
            PartyId = partyId;
        }

    }
}
