using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeStartOkNpcTradeMessage : Message
	{

		// Properties
		public int NpcId { get; set; }


		// Constructors
		public ExchangeStartOkNpcTradeMessage() { }

		public ExchangeStartOkNpcTradeMessage(int npcId = 0)
		{
			NpcId = npcId;
		}

	}
}
