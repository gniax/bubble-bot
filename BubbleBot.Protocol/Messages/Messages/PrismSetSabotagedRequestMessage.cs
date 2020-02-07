using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PrismSetSabotagedRequestMessage : Message
	{

		// Properties
		public uint SubAreaId { get; set; }


		// Constructors
		public PrismSetSabotagedRequestMessage() { }

		public PrismSetSabotagedRequestMessage(uint subAreaId = 0)
		{
			SubAreaId = subAreaId;
		}

	}
}
