using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class SetCharacterRestrictionsMessage : Message
	{

		// Properties
		public ActorRestrictionsInformations Restrictions { get; set; }


		// Constructors
		public SetCharacterRestrictionsMessage() { }

		public SetCharacterRestrictionsMessage(ActorRestrictionsInformations restrictions = null)
		{
			Restrictions = restrictions;
		}

	}
}
