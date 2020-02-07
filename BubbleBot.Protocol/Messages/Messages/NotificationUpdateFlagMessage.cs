using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class NotificationUpdateFlagMessage : Message
	{

		// Properties
		public uint Index { get; set; }


		// Constructors
		public NotificationUpdateFlagMessage() { }

		public NotificationUpdateFlagMessage(uint index = 0)
		{
			Index = index;
		}

	}
}
