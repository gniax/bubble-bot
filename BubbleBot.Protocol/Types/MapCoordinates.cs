using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class MapCoordinates
	{

		// Properties
		public int WorldX { get; set; }
		public int WorldY { get; set; }


		// Constructors
		public MapCoordinates() { }

		public MapCoordinates(int worldX = 0, int worldY = 0)
		{
			WorldX = worldX;
			WorldY = worldY;
		}

	}
}
