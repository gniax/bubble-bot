using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class LockableStateUpdateAbstractMessage : Message
	{

		// Properties
		public bool Locked { get; set; }


		// Constructors
		public LockableStateUpdateAbstractMessage() { }

		public LockableStateUpdateAbstractMessage(bool locked = false)
		{
			Locked = locked;
		}

	}
}
