using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ContactLookErrorMessage : Message
	{

		// Properties
		public uint RequestId { get; set; }


		// Constructors
		public ContactLookErrorMessage() { }

		public ContactLookErrorMessage(uint requestId = 0)
		{
			RequestId = requestId;
		}

	}
}
