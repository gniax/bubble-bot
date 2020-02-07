using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildPaddockRemovedMessage : Message
	{

		// Properties
		public int PaddockId { get; set; }


		// Constructors
		public GuildPaddockRemovedMessage() { }

		public GuildPaddockRemovedMessage(int paddockId = 0)
		{
			PaddockId = paddockId;
		}

	}
}
