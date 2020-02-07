using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class KrosmasterInventoryMessage : Message
	{

		// Properties
		public List<KrosmasterFigure> Figures { get; set; }


		// Constructors
		public KrosmasterInventoryMessage() { }

		public KrosmasterInventoryMessage(List<KrosmasterFigure> figures = null)
		{
			Figures = figures;
		}

	}
}
