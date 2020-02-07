using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class MountSetMessage : Message
	{

		// Properties
		public MountClientData MountData { get; set; }


		// Constructors
		public MountSetMessage() { }

		public MountSetMessage(MountClientData mountData = null)
		{
			MountData = mountData;
		}

	}
}
