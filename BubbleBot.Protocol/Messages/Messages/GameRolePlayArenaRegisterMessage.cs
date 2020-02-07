using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameRolePlayArenaRegisterMessage : Message
	{

		// Properties
		public uint BattleMode { get; set; }


		// Constructors
		public GameRolePlayArenaRegisterMessage() { }

		public GameRolePlayArenaRegisterMessage(uint battleMode = 3)
		{
			BattleMode = battleMode;
		}

	}
}
