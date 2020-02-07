using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameDataPaddockObjectRemoveMessage : Message
	{

		// Properties
		public uint CellId { get; set; }


		// Constructors
		public GameDataPaddockObjectRemoveMessage() { }

		public GameDataPaddockObjectRemoveMessage(uint cellId = 0)
		{
			CellId = cellId;
		}

	}
}
