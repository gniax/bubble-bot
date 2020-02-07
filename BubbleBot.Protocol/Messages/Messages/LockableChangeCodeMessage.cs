using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class LockableChangeCodeMessage : Message
	{

		// Properties
		public string Code { get; set; }


		// Constructors
		public LockableChangeCodeMessage() { }

		public LockableChangeCodeMessage(string code = "")
		{
			Code = code;
		}

	}
}
