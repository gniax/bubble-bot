using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameContextCreateMessage : Message
	{

		// Properties
		public uint Context { get; set; }


		// Constructors
		public GameContextCreateMessage() { }

		public GameContextCreateMessage(uint context = 1)
		{
			Context = context;
		}

	}
}
