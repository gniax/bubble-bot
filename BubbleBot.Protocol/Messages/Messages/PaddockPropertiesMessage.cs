using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PaddockPropertiesMessage : Message
	{

		// Properties
		public PaddockInformations Properties { get; set; }


		// Constructors
		public PaddockPropertiesMessage() { }

		public PaddockPropertiesMessage(PaddockInformations properties = null)
		{
			Properties = properties;
		}

	}
}
