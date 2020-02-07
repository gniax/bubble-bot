using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameDataPaddockObjectAddMessage : Message
	{

		// Properties
		public PaddockItem PaddockItemDescription { get; set; }


		// Constructors
		public GameDataPaddockObjectAddMessage() { }

		public GameDataPaddockObjectAddMessage(PaddockItem paddockItemDescription = null)
		{
			PaddockItemDescription = paddockItemDescription;
		}

	}
}
