using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeMultiCraftCrafterCanUseHisRessourcesMessage : Message
	{

		// Properties
		public bool Allowed { get; set; }


		// Constructors
		public ExchangeMultiCraftCrafterCanUseHisRessourcesMessage() { }

		public ExchangeMultiCraftCrafterCanUseHisRessourcesMessage(bool allowed = false)
		{
			Allowed = allowed;
		}

	}
}
