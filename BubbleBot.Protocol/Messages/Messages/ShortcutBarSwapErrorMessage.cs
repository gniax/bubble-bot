using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ShortcutBarSwapErrorMessage : Message
	{

		// Properties
		public uint Error { get; set; }


		// Constructors
		public ShortcutBarSwapErrorMessage() { }

		public ShortcutBarSwapErrorMessage(uint error = 0)
		{
			Error = error;
		}

	}
}
