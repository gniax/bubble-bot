using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class PrismsInfoValidMessage : Message
    {

        // Properties
        public List<PrismFightersInformation> Fights { get; set; }


        // Constructors
        public PrismsInfoValidMessage() { }

        public PrismsInfoValidMessage(List<PrismFightersInformation> fights = null)
        {
            Fights = fights;
        }

    }
}
