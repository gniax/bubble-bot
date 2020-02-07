using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class DocumentReadingBeginMessage : Message
	{

		// Properties
		public uint DocumentId { get; set; }


		// Constructors
		public DocumentReadingBeginMessage() { }

		public DocumentReadingBeginMessage(uint documentId = 0)
		{
			DocumentId = documentId;
		}

	}
}
