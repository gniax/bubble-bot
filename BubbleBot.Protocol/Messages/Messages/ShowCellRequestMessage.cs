using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ShowCellRequestMessage : Message
	{

		// Properties
		public uint CellId { get; set; }


		// Constructors
		public ShowCellRequestMessage() { }

		public ShowCellRequestMessage(uint cellId = 0)
		{
			CellId = cellId;
		}

	}
}
