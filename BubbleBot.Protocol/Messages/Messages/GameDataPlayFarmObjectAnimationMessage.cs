using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameDataPlayFarmObjectAnimationMessage : Message
	{

		// Properties
		public List<uint> CellId { get; set; }


		// Constructors
		public GameDataPlayFarmObjectAnimationMessage() { }

		public GameDataPlayFarmObjectAnimationMessage(List<uint> cellId = null)
		{
			CellId = cellId;
		}

	}
}
