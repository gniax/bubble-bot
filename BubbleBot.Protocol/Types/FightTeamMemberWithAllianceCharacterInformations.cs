using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class FightTeamMemberWithAllianceCharacterInformations : FightTeamMemberCharacterInformations
	{

		// Properties
		public BasicAllianceInformations AllianceInfos { get; set; }


		// Constructors
		public FightTeamMemberWithAllianceCharacterInformations() { }

		public FightTeamMemberWithAllianceCharacterInformations(int id = 0, string name = "", uint level = 0, BasicAllianceInformations allianceInfos = null)
		{
			Id = id;
			Name = name;
			Level = level;
			AllianceInfos = allianceInfos;
		}

	}
}
