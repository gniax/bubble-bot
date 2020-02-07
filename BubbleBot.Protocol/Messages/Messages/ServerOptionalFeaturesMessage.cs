using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ServerOptionalFeaturesMessage : Message
	{

		// Properties
		public List<uint> Features { get; set; }


		// Constructors
		public ServerOptionalFeaturesMessage() { }

		public ServerOptionalFeaturesMessage(List<uint> features = null)
		{
			Features = features;
		}

	}
}
