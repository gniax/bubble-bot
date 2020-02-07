using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameFightPlacementPositionRequestMessage : Message
	{

		// Properties
		public uint CellId { get; set; }


		// Constructors
		public GameFightPlacementPositionRequestMessage() { }

		public GameFightPlacementPositionRequestMessage(uint cellId = 0)
		{
			CellId = cellId;
		}

	}
}
