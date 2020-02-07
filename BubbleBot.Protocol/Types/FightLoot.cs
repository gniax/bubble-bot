using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class FightLoot
	{

		// Properties
		public List<uint> Objects { get; set; }
		public uint Kamas { get; set; }


		// Constructors
		public FightLoot() { }

		public FightLoot(uint kamas = 0, List<uint> objects = null)
		{
			Kamas = kamas;
			Objects = objects;
		}

	}
}
