using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class MapRunningFightListMessage : Message
	{

		// Properties
		public List<FightExternalInformations> Fights { get; set; }


		// Constructors
		public MapRunningFightListMessage() { }

		public MapRunningFightListMessage(List<FightExternalInformations> fights = null)
		{
			Fights = fights;
		}

	}
}
