using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class JobCrafterDirectoryAddMessage : Message
	{

		// Properties
		public JobCrafterDirectoryListEntry ListEntry { get; set; }


		// Constructors
		public JobCrafterDirectoryAddMessage() { }

		public JobCrafterDirectoryAddMessage(JobCrafterDirectoryListEntry listEntry = null)
		{
			ListEntry = listEntry;
		}

	}
}
