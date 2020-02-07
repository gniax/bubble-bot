using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeMountPaddockRemoveMessage : Message
	{

		// Properties
		public double MountId { get; set; }


		// Constructors
		public ExchangeMountPaddockRemoveMessage() { }

		public ExchangeMountPaddockRemoveMessage(double mountId = 0)
		{
			MountId = mountId;
		}

	}
}
