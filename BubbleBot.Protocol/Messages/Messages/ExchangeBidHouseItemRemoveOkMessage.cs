using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeBidHouseItemRemoveOkMessage : Message
	{

		// Properties
		public int SellerId { get; set; }


		// Constructors
		public ExchangeBidHouseItemRemoveOkMessage() { }

		public ExchangeBidHouseItemRemoveOkMessage(int sellerId = 0)
		{
			SellerId = sellerId;
		}

	}
}
