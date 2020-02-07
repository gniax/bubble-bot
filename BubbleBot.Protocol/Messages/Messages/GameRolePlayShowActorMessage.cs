using System.Collections.Generic;
using BubbleBot.Protocol.Types;
using Newtonsoft.Json;
using BubbleBot.Protocol.Converters;

namespace BubbleBot.Protocol.Messages
{
	public class GameRolePlayShowActorMessage : Message
	{

		// Properties
        [JsonConverter(typeof(TypedPropertyConverter))]
		public GameRolePlayActorInformations Informations { get; set; }


		// Constructors
		public GameRolePlayShowActorMessage() { }

		public GameRolePlayShowActorMessage(GameRolePlayActorInformations informations = null)
		{
			Informations = informations;
		}

	}
}
