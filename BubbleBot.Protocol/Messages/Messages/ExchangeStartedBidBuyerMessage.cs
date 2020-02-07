using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeStartedBidBuyerMessage : Message
	{

		// Properties
		public SellerBuyerDescriptor BuyerDescriptor { get; set; }


		// Constructors
		public ExchangeStartedBidBuyerMessage() { }

		public ExchangeStartedBidBuyerMessage(SellerBuyerDescriptor buyerDescriptor = null)
		{
			BuyerDescriptor = buyerDescriptor;
		}

	}
}
