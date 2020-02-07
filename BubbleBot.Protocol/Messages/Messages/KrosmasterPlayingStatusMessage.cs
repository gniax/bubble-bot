using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class KrosmasterPlayingStatusMessage : Message
	{

		// Properties
		public bool Playing { get; set; }


		// Constructors
		public KrosmasterPlayingStatusMessage() { }

		public KrosmasterPlayingStatusMessage(bool playing = false)
		{
			Playing = playing;
		}

	}
}
