using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class HouseSellFromInsideRequestMessage : HouseSellRequestMessage
	{

		// Constructors
		public HouseSellFromInsideRequestMessage() { }

		public HouseSellFromInsideRequestMessage(uint amount = 0)
		{
			Amount = amount;
		}

	}
}
