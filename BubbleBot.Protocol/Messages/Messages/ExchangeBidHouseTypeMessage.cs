using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeBidHouseTypeMessage : Message
	{

		// Properties
		public uint Type { get; set; }


		// Constructors
		public ExchangeBidHouseTypeMessage() { }

		public ExchangeBidHouseTypeMessage(uint type = 0)
		{
			Type = type;
		}

	}
}
