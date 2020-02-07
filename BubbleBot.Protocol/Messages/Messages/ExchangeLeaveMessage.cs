using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeLeaveMessage : LeaveDialogMessage
	{

		// Properties
		public bool Success { get; set; }


		// Constructors
		public ExchangeLeaveMessage() { }

		public ExchangeLeaveMessage(uint dialogType = 0, bool success = false)
		{
			DialogType = dialogType;
			Success = success;
		}

	}
}
