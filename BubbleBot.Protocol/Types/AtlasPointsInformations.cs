using System.Collections.Generic;

namespace BubbleBot.Protocol.Types
{
    public class AtlasPointsInformations
    {

        // Properties
        public List<MapCoordinatesExtended> Coords { get; set; }
        public uint Type { get; set; }


        // Constructors
        public AtlasPointsInformations() { }

        public AtlasPointsInformations(uint type = 0, List<MapCoordinatesExtended> coords = null)
        {
            Type = type;
            Coords = coords;
        }

    }
}
