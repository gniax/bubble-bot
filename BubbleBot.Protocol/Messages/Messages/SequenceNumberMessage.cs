using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class SequenceNumberMessage : Message
	{

		// Properties
		public uint Number { get; set; }


		// Constructors
		public SequenceNumberMessage() { }

		public SequenceNumberMessage(uint number = 0)
		{
			Number = number;
		}

	}
}
