using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ClientUIOpenedByObjectMessage : ClientUIOpenedMessage
	{

		// Properties
		public uint Uid { get; set; }


		// Constructors
		public ClientUIOpenedByObjectMessage() { }

		public ClientUIOpenedByObjectMessage(uint type = 0, uint uid = 0)
		{
			Type = type;
			Uid = uid;
		}

	}
}
