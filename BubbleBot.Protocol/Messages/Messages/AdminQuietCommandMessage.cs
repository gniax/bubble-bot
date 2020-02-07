using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AdminQuietCommandMessage : AdminCommandMessage
	{

		// Constructors
		public AdminQuietCommandMessage() { }

		public AdminQuietCommandMessage(string content = "")
		{
			Content = content;
		}

	}
}
