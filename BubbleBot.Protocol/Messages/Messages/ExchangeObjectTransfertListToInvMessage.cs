using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeObjectTransfertListToInvMessage : Message
	{

		// Properties
		public List<uint> Ids { get; set; }


		// Constructors
		public ExchangeObjectTransfertListToInvMessage() { }

		public ExchangeObjectTransfertListToInvMessage(List<uint> ids = null)
		{
			Ids = ids;
		}

	}
}
