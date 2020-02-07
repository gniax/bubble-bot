using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class StorageInventoryContentMessage : InventoryContentMessage
	{

		// Constructors
		public StorageInventoryContentMessage() { }

		public StorageInventoryContentMessage(uint kamas = 0, List<ObjectItem> objects = null)
		{
			Kamas = kamas;
			Objects = objects;
		}

	}
}
