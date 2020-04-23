using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeObjectTransfertListFromInvMessage : Message
    {

        // Properties
        public List<uint> Ids { get; set; }


        // Constructors
        public ExchangeObjectTransfertListFromInvMessage() { }

        public ExchangeObjectTransfertListFromInvMessage(List<uint> ids = null)
        {
            Ids = ids;
        }

    }
}
