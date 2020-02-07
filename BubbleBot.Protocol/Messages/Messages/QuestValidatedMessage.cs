using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class QuestValidatedMessage : Message
	{

		// Properties
		public uint QuestId { get; set; }


		// Constructors
		public QuestValidatedMessage() { }

		public QuestValidatedMessage(uint questId = 0)
		{
			QuestId = questId;
		}

	}
}
