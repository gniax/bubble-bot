using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class FighterStatsListMessage : Message
    {

        // Properties
        public CharacterCharacteristicsInformations Stats { get; set; }


        // Constructors
        public FighterStatsListMessage() { }

        public FighterStatsListMessage(CharacterCharacteristicsInformations stats = null)
        {
            Stats = stats;
        }

    }
}
