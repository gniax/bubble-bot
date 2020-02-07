using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class BasicSetAwayModeRequestMessage : Message
	{

		// Properties
		public bool Enable { get; set; }
		public bool Invisible { get; set; }


		// Constructors
		public BasicSetAwayModeRequestMessage() { }

		public BasicSetAwayModeRequestMessage(bool enable = false, bool invisible = false)
		{
			Enable = enable;
			Invisible = invisible;
		}

	}
}
