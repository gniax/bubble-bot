using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AcquaintanceSearchErrorMessage : Message
	{

		// Properties
		public uint Reason { get; set; }


		// Constructors
		public AcquaintanceSearchErrorMessage() { }

		public AcquaintanceSearchErrorMessage(uint reason = 0)
		{
			Reason = reason;
		}

	}
}
