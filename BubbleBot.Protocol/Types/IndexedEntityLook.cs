using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class IndexedEntityLook
	{

		// Properties
		public EntityLook Look { get; set; }
		public uint Index { get; set; }


		// Constructors
		public IndexedEntityLook() { }

		public IndexedEntityLook(EntityLook look = null, uint index = 0)
		{
			Look = look;
			Index = index;
		}

	}
}
