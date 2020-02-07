using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class ObjectEffectLadder : ObjectEffectCreature
	{

		// Properties
		public uint MonsterCount { get; set; }


		// Constructors
		public ObjectEffectLadder() { }

		public ObjectEffectLadder(uint actionId = 0, uint monsterFamilyId = 0, uint monsterCount = 0)
		{
			ActionId = actionId;
			MonsterFamilyId = monsterFamilyId;
			MonsterCount = monsterCount;
		}

	}
}
