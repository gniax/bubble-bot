using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeBuyMessage : Message
	{

		// Properties
		public uint ObjectToBuyId { get; set; }
		public uint Quantity { get; set; }


		// Constructors
		public ExchangeBuyMessage() { }

		public ExchangeBuyMessage(uint objectToBuyId = 0, uint quantity = 0)
		{
			ObjectToBuyId = objectToBuyId;
			Quantity = quantity;
		}

	}
}
