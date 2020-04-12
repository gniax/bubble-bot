using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class AllianceVersatileInfoListMessage : Message
    {

        // Properties
        public List<AllianceVersatileInformations> Alliances { get; set; }


        // Constructors
        public AllianceVersatileInfoListMessage() { }

        public AllianceVersatileInfoListMessage(List<AllianceVersatileInformations> alliances = null)
        {
            Alliances = alliances;
        }

    }
}
