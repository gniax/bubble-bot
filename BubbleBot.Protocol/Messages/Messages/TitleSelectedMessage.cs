using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class TitleSelectedMessage : Message
	{

		// Properties
		public uint TitleId { get; set; }


		// Constructors
		public TitleSelectedMessage() { }

		public TitleSelectedMessage(uint titleId = 0)
		{
			TitleId = titleId;
		}

	}
}
