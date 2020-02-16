using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class ObjectEffect
	{

		// Properties
		public uint ActionId { get; set; }
		public uint Value { get; set; }
		// Constructors
		public ObjectEffect() { }

		public ObjectEffect(uint actionId = 0, uint value = 0)
		{
			ActionId = actionId;
			Value = value;
		}

	}
}
