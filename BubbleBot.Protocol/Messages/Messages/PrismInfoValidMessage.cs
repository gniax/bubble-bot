using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PrismInfoValidMessage : Message
	{

		// Properties
		public ProtectedEntityWaitingForHelpInfo WaitingForHelpInfo { get; set; }


		// Constructors
		public PrismInfoValidMessage() { }

		public PrismInfoValidMessage(ProtectedEntityWaitingForHelpInfo waitingForHelpInfo = null)
		{
			WaitingForHelpInfo = waitingForHelpInfo;
		}

	}
}
