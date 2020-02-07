using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class OrnamentSelectErrorMessage : Message
	{

		// Properties
		public uint Reason { get; set; }


		// Constructors
		public OrnamentSelectErrorMessage() { }

		public OrnamentSelectErrorMessage(uint reason = 0)
		{
			Reason = reason;
		}

	}
}
