using System.Collections.Generic;
using BubbleBot.Protocol.Types;
using Newtonsoft.Json;
using BubbleBot.Protocol.Converters;

namespace BubbleBot.Protocol.Messages
{
	public class GameFightShowFighterMessage : Message
	{

		// Properties
        [JsonConverter(typeof(TypedPropertyConverter))]
		public GameFightFighterInformations Informations { get; set; }


		// Constructors
		public GameFightShowFighterMessage() { }

		public GameFightShowFighterMessage(GameFightFighterInformations informations = null)
		{
			Informations = informations;
		}

	}
}
