using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class StorageObjectUpdateMessage : Message
	{

		// Properties
		public ObjectItem @Object { get; set; }


		// Constructors
		public StorageObjectUpdateMessage() { }

		public StorageObjectUpdateMessage(ObjectItem @object = null)
		{
			@Object = @object;
		}

	}
}
