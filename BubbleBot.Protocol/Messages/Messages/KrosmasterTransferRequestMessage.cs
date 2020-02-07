using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class KrosmasterTransferRequestMessage : Message
	{

		// Properties
		public string Uid { get; set; }


		// Constructors
		public KrosmasterTransferRequestMessage() { }

		public KrosmasterTransferRequestMessage(string uid = "")
		{
			Uid = uid;
		}

	}
}
