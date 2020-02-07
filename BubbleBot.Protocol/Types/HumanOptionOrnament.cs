using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class HumanOptionOrnament : HumanOption
	{

		// Properties
		public uint OrnamentId { get; set; }


		// Constructors
		public HumanOptionOrnament() { }

		public HumanOptionOrnament(uint ornamentId = 0)
		{
			OrnamentId = ornamentId;
		}

	}
}
