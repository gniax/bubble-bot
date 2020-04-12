using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class KrosmasterInventoryMessage : Message
    {

        // Properties
        public List<KrosmasterFigure> Figures { get; set; }


        // Constructors
        public KrosmasterInventoryMessage() { }

        public KrosmasterInventoryMessage(List<KrosmasterFigure> figures = null)
        {
            Figures = figures;
        }

    }
}
