using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildFightPlayersEnemiesListMessage : Message
	{

		// Properties
		public List<CharacterMinimalPlusLookInformations> PlayerInfo { get; set; }
		public double FightId { get; set; }


		// Constructors
		public GuildFightPlayersEnemiesListMessage() { }

		public GuildFightPlayersEnemiesListMessage(double fightId = 0, List<CharacterMinimalPlusLookInformations> playerInfo = null)
		{
			FightId = fightId;
			PlayerInfo = playerInfo;
		}

	}
}
