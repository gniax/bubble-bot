using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ExchangeStartOkMountMessage : ExchangeStartOkMountWithOutPaddockMessage
    {

        // Properties
        public List<MountClientData> PaddockedMountsDescription { get; set; }


        // Constructors
        public ExchangeStartOkMountMessage() { }

        public ExchangeStartOkMountMessage(List<MountClientData> stabledMountsDescription = null, List<MountClientData> paddockedMountsDescription = null)
        {
            StabledMountsDescription = stabledMountsDescription;
            PaddockedMountsDescription = paddockedMountsDescription;
        }

    }
}
