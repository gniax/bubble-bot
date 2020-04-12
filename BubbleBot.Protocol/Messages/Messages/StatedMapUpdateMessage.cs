using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class StatedMapUpdateMessage : Message
    {

        // Properties
        public List<StatedElement> StatedElements { get; set; }


        // Constructors
        public StatedMapUpdateMessage() { }

        public StatedMapUpdateMessage(List<StatedElement> statedElements = null)
        {
            StatedElements = statedElements;
        }

    }
}
