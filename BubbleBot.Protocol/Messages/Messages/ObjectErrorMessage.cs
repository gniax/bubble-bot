using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ObjectErrorMessage : Message
	{

		// Properties
		public int Reason { get; set; }


		// Constructors
		public ObjectErrorMessage() { }

		public ObjectErrorMessage(int reason = 0)
		{
			Reason = reason;
		}

	}
}
