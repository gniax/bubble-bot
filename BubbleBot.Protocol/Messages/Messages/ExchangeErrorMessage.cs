using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeErrorMessage : Message
	{

		// Properties
		public int ErrorType { get; set; }


		// Constructors
		public ExchangeErrorMessage() { }

		public ExchangeErrorMessage(int errorType = 0)
		{
			ErrorType = errorType;
		}

	}
}
