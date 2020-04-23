namespace BubbleBot.Protocol.Messages
{
    public class PartyKickedByMessage : AbstractPartyMessage
    {

        // Properties
        public uint KickerId { get; set; }


        // Constructors
        public PartyKickedByMessage() { }

        public PartyKickedByMessage(uint partyId = 0, uint kickerId = 0)
        {
            PartyId = partyId;
            KickerId = kickerId;
        }

    }
}
