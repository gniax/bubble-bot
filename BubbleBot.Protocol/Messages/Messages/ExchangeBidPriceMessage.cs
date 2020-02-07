using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeBidPriceMessage : Message
	{

		// Properties
		public uint GenericId { get; set; }
		public int AveragePrice { get; set; }


		// Constructors
		public ExchangeBidPriceMessage() { }

		public ExchangeBidPriceMessage(uint genericId = 0, int averagePrice = 0)
		{
			GenericId = genericId;
			AveragePrice = averagePrice;
		}

	}
}
