using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class ObjectEffectString : ObjectEffect
	{

		// Properties
		public string Value { get; set; }


		// Constructors
		public ObjectEffectString() { }

		public ObjectEffectString(uint actionId = 0, string value = "")
		{
			ActionId = actionId;
			Value = value;
		}

	}
}
