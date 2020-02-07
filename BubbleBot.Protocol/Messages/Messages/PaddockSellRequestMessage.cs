using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PaddockSellRequestMessage : Message
	{

		// Properties
		public uint Price { get; set; }


		// Constructors
		public PaddockSellRequestMessage() { }

		public PaddockSellRequestMessage(uint price = 0)
		{
			Price = price;
		}

	}
}
