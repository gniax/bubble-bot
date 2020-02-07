using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class MimicryObjectAssociatedMessage : Message
	{

		// Properties
		public uint HostUID { get; set; }


		// Constructors
		public MimicryObjectAssociatedMessage() { }

		public MimicryObjectAssociatedMessage(uint hostUID = 0)
		{
			HostUID = hostUID;
		}

	}
}
