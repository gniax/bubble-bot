using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class IdentificationFailedMessage : Message
	{

		// Properties
		public uint Reason { get; set; }


		// Constructors
		public IdentificationFailedMessage() { }

		public IdentificationFailedMessage(uint reason = 99)
		{
			Reason = reason;
		}

	}
}
