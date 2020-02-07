using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class GoldItem : Item
	{

		// Properties
		public uint Sum { get; set; }


		// Constructors
		public GoldItem() { }

		public GoldItem(uint sum = 0)
		{
			Sum = sum;
		}

	}
}
