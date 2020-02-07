using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class TaxCollectorMovementAddMessage : Message
	{

		// Properties
		public TaxCollectorInformations Informations { get; set; }


		// Constructors
		public TaxCollectorMovementAddMessage() { }

		public TaxCollectorMovementAddMessage(TaxCollectorInformations informations = null)
		{
			Informations = informations;
		}

	}
}
