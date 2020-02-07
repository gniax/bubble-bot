using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameEntityDispositionMessage : Message
	{

		// Properties
		public IdentifiedEntityDispositionInformations Disposition { get; set; }


		// Constructors
		public GameEntityDispositionMessage() { }

		public GameEntityDispositionMessage(IdentifiedEntityDispositionInformations disposition = null)
		{
			Disposition = disposition;
		}

	}
}
