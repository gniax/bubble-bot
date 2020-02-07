using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AdminCommandMessage : Message
	{

		// Properties
		public string Content { get; set; }


		// Constructors
		public AdminCommandMessage() { }

		public AdminCommandMessage(string content = "")
		{
			Content = content;
		}

	}
}
