using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class SkillActionDescriptionTimed : SkillActionDescription
	{

		// Properties
		public uint Time { get; set; }


		// Constructors
		public SkillActionDescriptionTimed() { }

		public SkillActionDescriptionTimed(uint skillId = 0, uint time = 0)
		{
			SkillId = skillId;
			Time = time;
		}

	}
}
