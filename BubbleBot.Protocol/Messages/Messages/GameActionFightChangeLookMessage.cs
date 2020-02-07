using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameActionFightChangeLookMessage : AbstractGameActionMessage
	{

		// Properties
		public int TargetId { get; set; }
		public EntityLook EntityLook { get; set; }


		// Constructors
		public GameActionFightChangeLookMessage() { }

		public GameActionFightChangeLookMessage(uint actionId = 0, int sourceId = 0, int targetId = 0, EntityLook entityLook = null)
		{
			ActionId = actionId;
			SourceId = sourceId;
			TargetId = targetId;
			EntityLook = entityLook;
		}

	}
}
