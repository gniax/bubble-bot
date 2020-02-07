using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameFightOptionToggleMessage : Message
	{

		// Properties
		public uint Option { get; set; }


		// Constructors
		public GameFightOptionToggleMessage() { }

		public GameFightOptionToggleMessage(uint option = 3)
		{
			Option = option;
		}

	}
}
