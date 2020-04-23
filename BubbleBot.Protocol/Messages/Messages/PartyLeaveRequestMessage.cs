namespace BubbleBot.Protocol.Messages
{
    public class PartyLeaveRequestMessage : AbstractPartyMessage
    {

        // Constructors
        public PartyLeaveRequestMessage() { }

        public PartyLeaveRequestMessage(uint partyId = 0)
        {
            PartyId = partyId;
        }

    }
}
