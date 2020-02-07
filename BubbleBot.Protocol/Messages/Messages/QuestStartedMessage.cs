using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class QuestStartedMessage : Message
	{

		// Properties
		public uint QuestId { get; set; }


		// Constructors
		public QuestStartedMessage() { }

		public QuestStartedMessage(uint questId = 0)
		{
			QuestId = questId;
			
		}

	}
}
