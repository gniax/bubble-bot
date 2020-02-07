using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeCraftResultMessage : Message
	{

		// Properties
		public uint CraftResult { get; set; }


		// Constructors
		public ExchangeCraftResultMessage() { }

		public ExchangeCraftResultMessage(uint craftResult = 0)
		{
			CraftResult = craftResult;
		}

	}
}
