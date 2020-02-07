using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class Achievement
	{

		// Properties
		public List<AchievementObjective> FinishedObjective { get; set; }
		public List<AchievementStartedObjective> StartedObjectives { get; set; }
		public uint Id { get; set; }


		// Constructors
		public Achievement() { }

		public Achievement(uint id = 0, List<AchievementObjective> finishedObjective = null, List<AchievementStartedObjective> startedObjectives = null)
		{
			Id = id;
			FinishedObjective = finishedObjective;
			StartedObjectives = startedObjectives;
		}

	}
}
