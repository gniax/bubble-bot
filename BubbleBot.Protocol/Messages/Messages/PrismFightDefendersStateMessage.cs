using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class PrismFightDefendersStateMessage : Message
    {

        // Properties
        public List<CharacterMinimalPlusLookAndGradeInformations> MainFighters { get; set; }
        public List<CharacterMinimalPlusLookAndGradeInformations> ReserveFighters { get; set; }
        public double FightId { get; set; }


        // Constructors
        public PrismFightDefendersStateMessage() { }

        public PrismFightDefendersStateMessage(double fightId = 0, List<CharacterMinimalPlusLookAndGradeInformations> mainFighters = null, List<CharacterMinimalPlusLookAndGradeInformations> reserveFighters = null)
        {
            FightId = fightId;
            MainFighters = mainFighters;
            ReserveFighters = reserveFighters;
        }

    }
}
