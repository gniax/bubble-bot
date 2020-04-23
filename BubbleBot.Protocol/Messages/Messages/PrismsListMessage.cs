using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class PrismsListMessage : Message
    {

        // Properties
        public List<PrismSubareaEmptyInfo> Prisms { get; set; }


        // Constructors
        public PrismsListMessage() { }

        public PrismsListMessage(List<PrismSubareaEmptyInfo> prisms = null)
        {
            Prisms = prisms;
        }

    }
}
