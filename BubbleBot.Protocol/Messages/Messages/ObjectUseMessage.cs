using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ObjectUseMessage : Message
	{

		// Properties
		public uint ObjectUID { get; set; }


		// Constructors
		public ObjectUseMessage() { }

		public ObjectUseMessage(uint objectUID = 0)
		{
			ObjectUID = objectUID;
		}

	}
}
