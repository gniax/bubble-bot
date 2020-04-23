using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class InteractiveMapUpdateMessage : Message
    {

        // Properties
        public List<InteractiveElement> InteractiveElements { get; set; }


        // Constructors
        public InteractiveMapUpdateMessage() { }

        public InteractiveMapUpdateMessage(List<InteractiveElement> interactiveElements = null)
        {
            InteractiveElements = interactiveElements;
        }

    }
}
