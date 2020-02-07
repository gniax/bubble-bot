using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class SetEnableAVARequestMessage : Message
	{

		// Properties
		public bool Enable { get; set; }


		// Constructors
		public SetEnableAVARequestMessage() { }

		public SetEnableAVARequestMessage(bool enable = false)
		{
			Enable = enable;
		}

	}
}
