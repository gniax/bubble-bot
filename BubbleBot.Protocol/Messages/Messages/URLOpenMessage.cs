using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class URLOpenMessage : Message
	{

		// Properties
		public uint UrlId { get; set; }


		// Constructors
		public URLOpenMessage() { }

		public URLOpenMessage(uint urlId = 0)
		{
			UrlId = urlId;
		}

	}
}
