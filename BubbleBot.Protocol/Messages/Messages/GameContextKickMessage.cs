using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameContextKickMessage : Message
	{

		// Properties
		public int TargetId { get; set; }


		// Constructors
		public GameContextKickMessage() { }

		public GameContextKickMessage(int targetId = 0)
		{
			TargetId = targetId;
		}

	}
}
