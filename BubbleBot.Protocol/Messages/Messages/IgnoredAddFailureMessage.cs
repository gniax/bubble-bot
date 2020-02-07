using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class IgnoredAddFailureMessage : Message
	{

		// Properties
		public uint Reason { get; set; }


		// Constructors
		public IgnoredAddFailureMessage() { }

		public IgnoredAddFailureMessage(uint reason = 0)
		{
			Reason = reason;
		}

	}
}
