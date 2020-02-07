using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class QueueStatusMessage : Message
	{

		// Properties
		public uint Position { get; set; }
		public uint Total { get; set; }


		// Constructors
		public QueueStatusMessage() { }

		public QueueStatusMessage(uint position = 0, uint total = 0)
		{
			Position = position;
			Total = total;
		}

	}
}
