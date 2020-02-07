using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangePlayerMultiCraftRequestMessage : ExchangeRequestMessage
	{

		// Properties
		public uint Target { get; set; }
		public uint SkillId { get; set; }


		// Constructors
		public ExchangePlayerMultiCraftRequestMessage() { }

		public ExchangePlayerMultiCraftRequestMessage(int exchangeType = 0, uint target = 0, uint skillId = 0)
		{
			ExchangeType = exchangeType;
			Target = target;
			SkillId = skillId;
		}

	}
}
