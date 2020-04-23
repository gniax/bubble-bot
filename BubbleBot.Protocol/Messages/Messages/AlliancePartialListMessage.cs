using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class AlliancePartialListMessage : AllianceListMessage
    {

        // Constructors
        public AlliancePartialListMessage() { }

        public AlliancePartialListMessage(List<AllianceFactSheetInformations> alliances = null)
        {
            Alliances = alliances;
        }

    }
}
