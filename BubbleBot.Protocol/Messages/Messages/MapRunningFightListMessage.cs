using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class MapRunningFightListMessage : Message
    {

        // Properties
        public List<FightExternalInformations> Fights { get; set; }


        // Constructors
        public MapRunningFightListMessage() { }

        public MapRunningFightListMessage(List<FightExternalInformations> fights = null)
        {
            Fights = fights;
        }

    }
}
