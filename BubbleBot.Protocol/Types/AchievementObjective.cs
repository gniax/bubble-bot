using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class AchievementObjective
	{

		// Properties
		public uint Id { get; set; }
		public uint MaxValue { get; set; }


		// Constructors
		public AchievementObjective() { }

		public AchievementObjective(uint id = 0, uint maxValue = 0)
		{
			Id = id;
			MaxValue = maxValue;
		}

	}
}
