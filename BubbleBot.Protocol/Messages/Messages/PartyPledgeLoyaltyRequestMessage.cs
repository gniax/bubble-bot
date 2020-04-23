namespace BubbleBot.Protocol.Messages
{
    public class PartyPledgeLoyaltyRequestMessage : AbstractPartyMessage
    {

        // Properties
        public bool Loyal { get; set; }


        // Constructors
        public PartyPledgeLoyaltyRequestMessage() { }

        public PartyPledgeLoyaltyRequestMessage(uint partyId = 0, bool loyal = false)
        {
            PartyId = partyId;
            Loyal = loyal;
        }

    }
}
