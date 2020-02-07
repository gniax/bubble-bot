using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class MapCoordinatesExtended : MapCoordinatesAndId
	{

		// Properties
		public uint SubAreaId { get; set; }


		// Constructors
		public MapCoordinatesExtended() { }

		public MapCoordinatesExtended(int worldX = 0, int worldY = 0, int mapId = 0, uint subAreaId = 0)
		{
			WorldX = worldX;
			WorldY = worldY;
			MapId = mapId;
			SubAreaId = subAreaId;
		}

	}
}
