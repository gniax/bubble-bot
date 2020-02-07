using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameFightRefreshFighterMessage : Message
	{

		// Properties
		public GameContextActorInformations Informations { get; set; }


		// Constructors
		public GameFightRefreshFighterMessage() { }

		public GameFightRefreshFighterMessage(GameContextActorInformations informations = null)
		{
			Informations = informations;
		}

	}
}
