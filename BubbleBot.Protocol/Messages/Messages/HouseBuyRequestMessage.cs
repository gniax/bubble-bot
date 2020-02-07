using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class HouseBuyRequestMessage : Message
	{

		// Properties
		public uint ProposedPrice { get; set; }


		// Constructors
		public HouseBuyRequestMessage() { }

		public HouseBuyRequestMessage(uint proposedPrice = 0)
		{
			ProposedPrice = proposedPrice;
		}

	}
}
