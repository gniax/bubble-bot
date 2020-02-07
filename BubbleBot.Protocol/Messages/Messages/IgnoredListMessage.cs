using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class IgnoredListMessage : Message
	{

		// Properties
		public List<IgnoredInformations> IgnoredList { get; set; }


		// Constructors
		public IgnoredListMessage() { }

		public IgnoredListMessage(List<IgnoredInformations> ignoredList = null)
		{
			IgnoredList = ignoredList;
		}

	}
}
