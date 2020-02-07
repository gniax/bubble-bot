using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameFightSynchronizeMessage : Message
	{

		// Properties
		public List<GameFightFighterInformations> Fighters { get; set; }


		// Constructors
		public GameFightSynchronizeMessage() { }

		public GameFightSynchronizeMessage(List<GameFightFighterInformations> fighters = null)
		{
			Fighters = fighters;
		}

	}
}
