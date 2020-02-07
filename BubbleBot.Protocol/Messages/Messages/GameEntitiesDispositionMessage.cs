using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameEntitiesDispositionMessage : Message
	{

		// Properties
		public List<IdentifiedEntityDispositionInformations> Dispositions { get; set; }


		// Constructors
		public GameEntitiesDispositionMessage() { }

		public GameEntitiesDispositionMessage(List<IdentifiedEntityDispositionInformations> dispositions = null)
		{
			Dispositions = dispositions;
		}

	}
}
