using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeMountStableRemoveMessage : Message
	{

		// Properties
		public double MountId { get; set; }


		// Constructors
		public ExchangeMountStableRemoveMessage() { }

		public ExchangeMountStableRemoveMessage(double mountId = 0)
		{
			MountId = mountId;
		}

	}
}
