using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GameFightSpectateMessage : Message
    {

        // Properties
        public List<FightDispellableEffectExtendedInformations> Effects { get; set; }
        public List<GameActionMark> Marks { get; set; }
        public uint GameTurn { get; set; }


        // Constructors
        public GameFightSpectateMessage() { }

        public GameFightSpectateMessage(uint gameTurn = 0, List<FightDispellableEffectExtendedInformations> effects = null, List<GameActionMark> marks = null)
        {
            GameTurn = gameTurn;
            Effects = effects;
            Marks = marks;
        }

    }
}
