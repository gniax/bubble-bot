using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class TeleportBuddiesAnswerMessage : Message
	{

		// Properties
		public bool Accept { get; set; }


		// Constructors
		public TeleportBuddiesAnswerMessage() { }

		public TeleportBuddiesAnswerMessage(bool accept = false)
		{
			Accept = accept;
		}

	}
}
