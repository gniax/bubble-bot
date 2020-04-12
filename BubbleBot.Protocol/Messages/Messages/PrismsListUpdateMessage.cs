using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class PrismsListUpdateMessage : PrismsListMessage
    {

        // Constructors
        public PrismsListUpdateMessage() { }

        public PrismsListUpdateMessage(List<PrismSubareaEmptyInfo> prisms = null)
        {
            Prisms = prisms;
        }

    }
}
