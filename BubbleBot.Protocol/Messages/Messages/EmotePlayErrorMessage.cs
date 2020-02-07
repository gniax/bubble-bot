using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class EmotePlayErrorMessage : Message
	{

		// Properties
		public uint EmoteId { get; set; }


		// Constructors
		public EmotePlayErrorMessage() { }

		public EmotePlayErrorMessage(uint emoteId = 0)
		{
			EmoteId = emoteId;
		}

	}
}
