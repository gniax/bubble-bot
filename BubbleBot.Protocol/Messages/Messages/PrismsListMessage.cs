using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PrismsListMessage : Message
	{

		// Properties
		public List<PrismSubareaEmptyInfo> Prisms { get; set; }


		// Constructors
		public PrismsListMessage() { }

		public PrismsListMessage(List<PrismSubareaEmptyInfo> prisms = null)
		{
			Prisms = prisms;
		}

	}
}
