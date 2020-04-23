using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class MapObstacleUpdateMessage : Message
    {

        // Properties
        public List<MapObstacle> Obstacles { get; set; }


        // Constructors
        public MapObstacleUpdateMessage() { }

        public MapObstacleUpdateMessage(List<MapObstacle> obstacles = null)
        {
            Obstacles = obstacles;
        }

    }
}
