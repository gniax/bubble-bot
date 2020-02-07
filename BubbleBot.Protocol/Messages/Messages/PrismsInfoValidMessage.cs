using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PrismsInfoValidMessage : Message
	{

		// Properties
		public List<PrismFightersInformation> Fights { get; set; }


		// Constructors
		public PrismsInfoValidMessage() { }

		public PrismsInfoValidMessage(List<PrismFightersInformation> fights = null)
		{
			Fights = fights;
		}

	}
}
