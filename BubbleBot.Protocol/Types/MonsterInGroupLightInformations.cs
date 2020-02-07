using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class MonsterInGroupLightInformations
	{

		// Properties
		public int CreatureGenericId { get; set; }
		public uint Grade { get; set; }


		// Constructors
		public MonsterInGroupLightInformations() { }

		public MonsterInGroupLightInformations(int creatureGenericId = 0, uint grade = 0)
		{
			CreatureGenericId = creatureGenericId;
			Grade = grade;
		}

	}
}
