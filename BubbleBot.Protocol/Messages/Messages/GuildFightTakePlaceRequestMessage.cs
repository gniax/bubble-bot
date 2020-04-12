namespace BubbleBot.Protocol.Messages
{
    public class GuildFightTakePlaceRequestMessage : GuildFightJoinRequestMessage
    {

        // Properties
        public int ReplacedCharacterId { get; set; }


        // Constructors
        public GuildFightTakePlaceRequestMessage() { }

        public GuildFightTakePlaceRequestMessage(int taxCollectorId = 0, int replacedCharacterId = 0)
        {
            TaxCollectorId = taxCollectorId;
            ReplacedCharacterId = replacedCharacterId;
        }

    }
}
