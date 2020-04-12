namespace BubbleBot.Protocol.Messages
{
    public class PartyRefuseInvitationMessage : AbstractPartyMessage
    {

        // Constructors
        public PartyRefuseInvitationMessage() { }

        public PartyRefuseInvitationMessage(uint partyId = 0)
        {
            PartyId = partyId;
        }

    }
}
