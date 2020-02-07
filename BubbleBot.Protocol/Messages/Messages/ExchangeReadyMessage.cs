using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeReadyMessage : Message
	{

		// Properties
		public bool Ready { get; set; }
		public uint Step { get; set; }


		// Constructors
		public ExchangeReadyMessage() { }

		public ExchangeReadyMessage(bool ready = false, uint step = 0)
		{
			Ready = ready;
			Step = step;
		}

	}
}
