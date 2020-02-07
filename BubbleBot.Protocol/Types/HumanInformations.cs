using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class HumanInformations
	{

		// Properties
		public List<HumanOption> Options { get; set; }
		public ActorRestrictionsInformations Restrictions { get; set; }
		public bool Sex { get; set; }


		// Constructors
		public HumanInformations() { }

		public HumanInformations(ActorRestrictionsInformations restrictions = null, bool sex = false, List<HumanOption> options = null)
		{
			Restrictions = restrictions;
			Sex = sex;
			Options = options;
		}

	}
}
