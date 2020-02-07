using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeItemAutoCraftStopedMessage : Message
	{

		// Properties
		public int Reason { get; set; }


		// Constructors
		public ExchangeItemAutoCraftStopedMessage() { }

		public ExchangeItemAutoCraftStopedMessage(int reason = 0)
		{
			Reason = reason;
		}

	}
}
