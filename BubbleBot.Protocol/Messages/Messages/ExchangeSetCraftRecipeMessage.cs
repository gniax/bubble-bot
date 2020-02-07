using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeSetCraftRecipeMessage : Message
	{

		// Properties
		public uint ObjectGID { get; set; }


		// Constructors
		public ExchangeSetCraftRecipeMessage() { }

		public ExchangeSetCraftRecipeMessage(uint objectGID = 0)
		{
			ObjectGID = objectGID;
		}

	}
}
