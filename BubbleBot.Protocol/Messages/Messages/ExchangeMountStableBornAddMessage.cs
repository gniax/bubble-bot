using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeMountStableBornAddMessage : ExchangeMountStableAddMessage
	{

		// Constructors
		public ExchangeMountStableBornAddMessage() { }

		public ExchangeMountStableBornAddMessage(MountClientData mountDescription = null)
		{
			MountDescription = mountDescription;
		}

	}
}
