using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ObjectsDeletedMessage : Message
	{

		// Properties
		public List<uint> ObjectUID { get; set; }


		// Constructors
		public ObjectsDeletedMessage() { }

		public ObjectsDeletedMessage(List<uint> objectUID = null)
		{
			ObjectUID = objectUID;
		}

	}
}
