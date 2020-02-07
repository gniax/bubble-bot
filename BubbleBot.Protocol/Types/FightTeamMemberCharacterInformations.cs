using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class FightTeamMemberCharacterInformations : FightTeamMemberInformations
	{

		// Properties
		public string Name { get; set; }
		public uint Level { get; set; }


		// Constructors
		public FightTeamMemberCharacterInformations() { }

		public FightTeamMemberCharacterInformations(int id = 0, string name = "", uint level = 0)
		{
			Id = id;
			Name = name;
			Level = level;
		}

	}
}
