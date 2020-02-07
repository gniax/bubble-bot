using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class QuestStepInfoRequestMessage : Message
	{

		// Properties
		public uint QuestId { get; set; }


		// Constructors
		public QuestStepInfoRequestMessage() { }

		public QuestStepInfoRequestMessage(uint questId = 0)
		{
			QuestId = questId;
		}

	}
}
