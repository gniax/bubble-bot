using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class IdentificationFailedForBadVersionMessage : IdentificationFailedMessage
	{

		// Constructors
		public IdentificationFailedForBadVersionMessage() { }

		public IdentificationFailedForBadVersionMessage(uint reason = 99)
		{
			Reason = reason;
		}

	}
}
