namespace BubbleBot.Protocol.Messages
{
    public class PartyInvitationCancelledForGuestMessage : AbstractPartyMessage
    {

        // Properties
        public uint CancelerId { get; set; }


        // Constructors
        public PartyInvitationCancelledForGuestMessage() { }

        public PartyInvitationCancelledForGuestMessage(uint partyId = 0, uint cancelerId = 0)
        {
            PartyId = partyId;
            CancelerId = cancelerId;
        }

    }
}
