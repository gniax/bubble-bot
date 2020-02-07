using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class NicknameRefusedMessage : Message
	{

		// Properties
		public uint Reason { get; set; }


		// Constructors
		public NicknameRefusedMessage() { }

		public NicknameRefusedMessage(uint reason = 99)
		{
			Reason = reason;
		}

	}
}
