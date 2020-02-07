using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class StatsUpgradeRequestMessage : Message
	{

		// Properties
		public uint StatId { get; set; }
		public uint BoostPoint { get; set; }


		// Constructors
		public StatsUpgradeRequestMessage() { }

		public StatsUpgradeRequestMessage(uint statId = 11, uint boostPoint = 0)
		{
			StatId = statId;
			BoostPoint = boostPoint;
		}

	}
}
