using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PaddockToSellListRequestMessage : Message
	{

		// Properties
		public uint PageIndex { get; set; }


		// Constructors
		public PaddockToSellListRequestMessage() { }

		public PaddockToSellListRequestMessage(uint pageIndex = 0)
		{
			PageIndex = pageIndex;
		}

	}
}
