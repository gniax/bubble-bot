using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class PartyUpdateMessage : AbstractPartyEventMessage
    {

        // Properties
        public PartyMemberInformations MemberInformations { get; set; }


        // Constructors
        public PartyUpdateMessage() { }

        public PartyUpdateMessage(uint partyId = 0, PartyMemberInformations memberInformations = null)
        {
            PartyId = partyId;
            MemberInformations = memberInformations;
        }

    }
}
