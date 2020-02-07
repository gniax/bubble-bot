using System.Collections.Generic;

namespace BubbleBot.Protocol.Data.Maps
{
    public class Map
    {

        // Properties
        public int Id { get; set; }
        public long TopNeighbourId { get; set; }
        public long BottomNeighbourId { get; set; }
        public long LeftNeighbourId { get; set; }
        public long RightNeighbourId { get; set; }
        public List<Cell> Cells { get; set; }
        public Dictionary<short, List<GraphicalElement>> MidgroundLayer { get; set; }


        // Constructor
        internal Map() { }

    }
}
