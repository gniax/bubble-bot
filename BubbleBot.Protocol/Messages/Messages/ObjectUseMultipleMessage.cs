using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ObjectUseMultipleMessage : ObjectUseMessage
	{

		// Properties
		public uint Quantity { get; set; }


		// Constructors
		public ObjectUseMultipleMessage() { }

		public ObjectUseMultipleMessage(uint objectUID = 0, uint quantity = 0)
		{
			ObjectUID = objectUID;
			Quantity = quantity;
		}

	}
}
