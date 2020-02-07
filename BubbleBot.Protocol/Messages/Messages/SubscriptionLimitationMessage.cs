using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class SubscriptionLimitationMessage : Message
	{

		// Properties
		public uint Reason { get; set; }


		// Constructors
		public SubscriptionLimitationMessage() { }

		public SubscriptionLimitationMessage(uint reason = 0)
		{
			Reason = reason;
		}

	}
}
