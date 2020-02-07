using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class InteractiveMapUpdateMessage : Message
	{

		// Properties
		public List<InteractiveElement> InteractiveElements { get; set; }


		// Constructors
		public InteractiveMapUpdateMessage() { }

		public InteractiveMapUpdateMessage(List<InteractiveElement> interactiveElements = null)
		{
			InteractiveElements = interactiveElements;
		}

	}
}
