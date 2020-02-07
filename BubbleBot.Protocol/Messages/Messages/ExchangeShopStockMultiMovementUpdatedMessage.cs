using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeShopStockMultiMovementUpdatedMessage : Message
	{

		// Properties
		public List<ObjectItemToSell> ObjectInfoList { get; set; }


		// Constructors
		public ExchangeShopStockMultiMovementUpdatedMessage() { }

		public ExchangeShopStockMultiMovementUpdatedMessage(List<ObjectItemToSell> objectInfoList = null)
		{
			ObjectInfoList = objectInfoList;
		}

	}
}
