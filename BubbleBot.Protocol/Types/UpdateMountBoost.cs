using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class UpdateMountBoost
	{

		// Properties
		public int Type { get; set; }


		// Constructors
		public UpdateMountBoost() { }

		public UpdateMountBoost(int type = 0)
		{
			Type = type;
		}

	}
}
