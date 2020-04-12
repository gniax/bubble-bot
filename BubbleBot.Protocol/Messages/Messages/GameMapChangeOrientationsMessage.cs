using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GameMapChangeOrientationsMessage : Message
    {

        // Properties
        public List<ActorOrientation> Orientations { get; set; }


        // Constructors
        public GameMapChangeOrientationsMessage() { }

        public GameMapChangeOrientationsMessage(List<ActorOrientation> orientations = null)
        {
            Orientations = orientations;
        }

    }
}
