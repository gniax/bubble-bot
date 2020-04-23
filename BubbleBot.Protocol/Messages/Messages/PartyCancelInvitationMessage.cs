namespace BubbleBot.Protocol.Messages
{
    public class PartyCancelInvitationMessage : AbstractPartyMessage
    {

        // Properties
        public uint GuestId { get; set; }


        // Constructors
        public PartyCancelInvitationMessage() { }

        public PartyCancelInvitationMessage(uint partyId = 0, uint guestId = 0)
        {
            PartyId = partyId;
            GuestId = guestId;
        }

    }
}
