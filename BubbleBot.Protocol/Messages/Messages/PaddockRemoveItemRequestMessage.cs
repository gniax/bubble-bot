using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PaddockRemoveItemRequestMessage : Message
	{

		// Properties
		public uint CellId { get; set; }


		// Constructors
		public PaddockRemoveItemRequestMessage() { }

		public PaddockRemoveItemRequestMessage(uint cellId = 0)
		{
			CellId = cellId;
		}

	}
}
