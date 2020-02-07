using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ChatAbstractClientMessage : Message
	{

		// Properties
		public string Content { get; set; }


		// Constructors
		public ChatAbstractClientMessage() { }

		public ChatAbstractClientMessage(string content = "")
		{
			Content = content;
		}

	}
}
