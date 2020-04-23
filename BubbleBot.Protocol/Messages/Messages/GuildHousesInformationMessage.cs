using BubbleBot.Protocol.Types;
using System.Collections.Generic;

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
