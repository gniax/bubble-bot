using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class LifePointsRegenBeginMessage : Message
	{

		// Properties
		public uint RegenRate { get; set; }


		// Constructors
		public LifePointsRegenBeginMessage() { }

		public LifePointsRegenBeginMessage(uint regenRate = 0)
		{
			RegenRate = regenRate;
		}

	}
}
