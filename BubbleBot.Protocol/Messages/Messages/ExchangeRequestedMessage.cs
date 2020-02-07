using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeRequestedMessage : Message
	{

		// Properties
		public int ExchangeType { get; set; }


		// Constructors
		public ExchangeRequestedMessage() { }

		public ExchangeRequestedMessage(int exchangeType = 0)
		{
			ExchangeType = exchangeType;
		}

	}
}
