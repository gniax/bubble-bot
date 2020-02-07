using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeObjectTransfertListFromInvMessage : Message
	{

		// Properties
		public List<uint> Ids { get; set; }


		// Constructors
		public ExchangeObjectTransfertListFromInvMessage() { }

		public ExchangeObjectTransfertListFromInvMessage(List<uint> ids = null)
		{
			Ids = ids;
		}

	}
}
