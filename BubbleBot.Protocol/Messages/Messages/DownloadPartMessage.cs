using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class DownloadPartMessage : Message
	{

		// Properties
		public string Id { get; set; }


		// Constructors
		public DownloadPartMessage() { }

		public DownloadPartMessage(string id = "")
		{
			Id = id;
		}

	}
}
