namespace BubbleBot.Protocol.Messages
{
    public class PartyStopFollowRequestMessage : AbstractPartyMessage
    {

        // Constructors
        public PartyStopFollowRequestMessage() { }

        public PartyStopFollowRequestMessage(uint partyId = 0)
        {
            PartyId = partyId;
        }

    }
}
