namespace BubbleBot.Protocol.Messages
{
    public class PartyInvitationDetailsRequestMessage : AbstractPartyMessage
    {

        // Constructors
        public PartyInvitationDetailsRequestMessage() { }

        public PartyInvitationDetailsRequestMessage(uint partyId = 0)
        {
            PartyId = partyId;
        }

    }
}
