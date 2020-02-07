using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeBidHouseGenericItemRemovedMessage : Message
	{

		// Properties
		public int ObjGenericId { get; set; }


		// Constructors
		public ExchangeBidHouseGenericItemRemovedMessage() { }

		public ExchangeBidHouseGenericItemRemovedMessage(int objGenericId = 0)
		{
			ObjGenericId = objGenericId;
		}

	}
}
