using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class AbstractCharacterInformation
	{

		// Properties
		public uint Id { get; set; }


		// Constructors
		public AbstractCharacterInformation() { }

		public AbstractCharacterInformation(uint id = 0)
		{
			Id = id;
		}

	}
}
