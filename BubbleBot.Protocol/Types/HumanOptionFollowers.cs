using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class HumanOptionFollowers : HumanOption
	{

		// Properties
		public List<IndexedEntityLook> FollowingCharactersLook { get; set; }


		// Constructors
		public HumanOptionFollowers() { }

		public HumanOptionFollowers(List<IndexedEntityLook> followingCharactersLook = null)
		{
			FollowingCharactersLook = followingCharactersLook;
		}

	}
}
