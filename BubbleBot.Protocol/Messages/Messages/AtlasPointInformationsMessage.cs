using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AtlasPointInformationsMessage : Message
	{

		// Properties
		public AtlasPointsInformations Type { get; set; }


		// Constructors
		public AtlasPointInformationsMessage() { }

		public AtlasPointInformationsMessage(AtlasPointsInformations type = null)
		{
			Type = type;
		}

	}
}
