using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GuildHousesInformationMessage : Message
	{

		// Properties
		public List<HouseInformationsForGuild> HousesInformations { get; set; }


		// Constructors
		public GuildHousesInformationMessage() { }

		public GuildHousesInformationMessage(List<HouseInformationsForGuild> housesInformations = null)
		{
			HousesInformations = housesInformations;
		}

	}
}
