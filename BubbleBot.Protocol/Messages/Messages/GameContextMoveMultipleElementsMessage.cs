using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GameContextMoveMultipleElementsMessage : Message
    {

        // Properties
        public List<EntityMovementInformations> Movements { get; set; }


        // Constructors
        public GameContextMoveMultipleElementsMessage() { }

        public GameContextMoveMultipleElementsMessage(List<EntityMovementInformations> movements = null)
        {
            Movements = movements;
        }

    }
}
