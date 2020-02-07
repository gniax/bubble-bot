using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeObjectMoveKamaMessage : Message
	{

		// Properties
		public int Quantity { get; set; }


		// Constructors
		public ExchangeObjectMoveKamaMessage() { }

		public ExchangeObjectMoveKamaMessage(int quantity = 0)
		{
			Quantity = quantity;
		}

	}
}
