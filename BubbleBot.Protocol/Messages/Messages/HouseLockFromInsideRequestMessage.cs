using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class HouseLockFromInsideRequestMessage : LockableChangeCodeMessage
	{

		// Constructors
		public HouseLockFromInsideRequestMessage() { }

		public HouseLockFromInsideRequestMessage(string code = "")
		{
			Code = code;
		}

	}
}
