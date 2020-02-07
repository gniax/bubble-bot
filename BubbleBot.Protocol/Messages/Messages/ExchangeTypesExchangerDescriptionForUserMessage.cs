using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeTypesExchangerDescriptionForUserMessage : Message
	{

		// Properties
		public List<uint> TypeDescription { get; set; }


		// Constructors
		public ExchangeTypesExchangerDescriptionForUserMessage() { }

		public ExchangeTypesExchangerDescriptionForUserMessage(List<uint> typeDescription = null)
		{
			TypeDescription = typeDescription;
		}

	}
}
