using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class KrosmasterInventoryErrorMessage : Message
	{

		// Properties
		public uint Reason { get; set; }


		// Constructors
		public KrosmasterInventoryErrorMessage() { }

		public KrosmasterInventoryErrorMessage(uint reason = 0)
		{
			Reason = reason;
		}

	}
}
