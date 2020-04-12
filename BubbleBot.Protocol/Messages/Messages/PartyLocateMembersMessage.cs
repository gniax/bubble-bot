using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class PartyLocateMembersMessage : AbstractPartyMessage
    {

        // Properties
        public List<PartyMemberGeoPosition> Geopositions { get; set; }


        // Constructors
        public PartyLocateMembersMessage() { }

        public PartyLocateMembersMessage(uint partyId = 0, List<PartyMemberGeoPosition> geopositions = null)
        {
            PartyId = partyId;
            Geopositions = geopositions;
        }

    }
}
