using System.Collections.Generic;
using BubbleBot.Protocol.Converters;
using BubbleBot.Protocol.Types;
using Newtonsoft.Json;

namespace BubbleBot.Protocol.Messages
{
	public class QuestStepInfoMessage : Message
	{

		// Properties
        [JsonConverter(typeof(TypedPropertyConverter))]
		public QuestActiveInformations Infos { get; set; }


		// Constructors
		public QuestStepInfoMessage() { }

		public QuestStepInfoMessage(QuestActiveInformations infos = null)
		{
			Infos = infos;
		}

	}
}
