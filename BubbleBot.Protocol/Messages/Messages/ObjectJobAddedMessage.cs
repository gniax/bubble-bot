using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ObjectJobAddedMessage : Message
	{

		// Properties
		public uint JobId { get; set; }


		// Constructors
		public ObjectJobAddedMessage() { }

		public ObjectJobAddedMessage(uint jobId = 0)
		{
			JobId = jobId;
		}

	}
}
