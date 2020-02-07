using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ServerSelectionMessage : Message
	{

		// Properties
		public int ServerId { get; set; }


		// Constructors
		public ServerSelectionMessage() { }

		public ServerSelectionMessage(int serverId = 0)
		{
			ServerId = serverId;
		}

	}
}
