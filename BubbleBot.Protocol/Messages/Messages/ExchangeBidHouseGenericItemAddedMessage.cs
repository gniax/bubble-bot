using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeBidHouseGenericItemAddedMessage : Message
	{

		// Properties
		public int ObjGenericId { get; set; }


		// Constructors
		public ExchangeBidHouseGenericItemAddedMessage() { }

		public ExchangeBidHouseGenericItemAddedMessage(int objGenericId = 0)
		{
			ObjGenericId = objGenericId;
		}

	}
}
