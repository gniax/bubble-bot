using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class ServerSessionConstantInteger : ServerSessionConstant
	{

		// Properties
		public int Value { get; set; }


		// Constructors
		public ServerSessionConstantInteger() { }

		public ServerSessionConstantInteger(uint id = 0, int value = 0)
		{
			Id = id;
			Value = value;
		}

	}
}
