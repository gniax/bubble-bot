using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class GroupMonsterStaticInformations
	{

		// Properties
		public List<MonsterInGroupInformations> Underlings { get; set; }
		public MonsterInGroupLightInformations MainCreatureLightInfos { get; set; }


		// Constructors
		public GroupMonsterStaticInformations() { }

		public GroupMonsterStaticInformations(MonsterInGroupLightInformations mainCreatureLightInfos = null, List<MonsterInGroupInformations> underlings = null)
		{
			MainCreatureLightInfos = mainCreatureLightInfos;
			Underlings = underlings;
		}

	}
}
