using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ObjectsAddedMessage : Message
	{

		// Properties
		public List<ObjectItem> @Object { get; set; }


		// Constructors
		public ObjectsAddedMessage() { }

		public ObjectsAddedMessage(List<ObjectItem> @object = null)
		{
			@Object = @object;
		}

	}
}
