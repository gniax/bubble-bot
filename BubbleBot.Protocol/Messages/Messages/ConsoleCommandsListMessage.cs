using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ConsoleCommandsListMessage : Message
	{

		// Properties
		public List<string> Aliases { get; set; }
		public List<string> Args { get; set; }
		public List<string> Descriptions { get; set; }


		// Constructors
		public ConsoleCommandsListMessage() { }

		public ConsoleCommandsListMessage(List<string> aliases = null, List<string> args = null, List<string> descriptions = null)
		{
			Aliases = aliases;
			Args = args;
			Descriptions = descriptions;
		}

	}
}
