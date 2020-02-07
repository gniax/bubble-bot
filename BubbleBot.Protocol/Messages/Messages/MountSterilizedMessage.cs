using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class MountSterilizedMessage : Message
	{

		// Properties
		public double MountId { get; set; }


		// Constructors
		public MountSterilizedMessage() { }

		public MountSterilizedMessage(double mountId = 0)
		{
			MountId = mountId;
		}

	}
}
