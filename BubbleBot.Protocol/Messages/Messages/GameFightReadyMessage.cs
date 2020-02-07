using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameFightReadyMessage : Message
	{

		// Properties
		public bool IsReady { get; set; }


		// Constructors
		public GameFightReadyMessage() { }

		public GameFightReadyMessage(bool isReady = false)
		{
			IsReady = isReady;
		}

	}
}
