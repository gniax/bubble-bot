using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class UpdateMountIntBoost : UpdateMountBoost
	{

		// Properties
		public int Value { get; set; }


		// Constructors
		public UpdateMountIntBoost() { }

		public UpdateMountIntBoost(int type = 0, int value = 0)
		{
			Type = type;
			Value = value;
		}

	}
}
