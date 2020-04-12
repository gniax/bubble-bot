namespace BubbleBot.Protocol.Messages
{
    public class PartyCannotJoinErrorMessage : AbstractPartyMessage
    {

        // Properties
        public uint Reason { get; set; }


        // Constructors
        public PartyCannotJoinErrorMessage() { }

        public PartyCannotJoinErrorMessage(uint partyId = 0, uint reason = 0)
        {
            PartyId = partyId;
            Reason = reason;
        }

    }
}
