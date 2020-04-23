using System.Collections.Generic;

namespace BubbleBot.Protocol.Types
{
    public class GameRolePlayNpcQuestFlag
    {

        // Properties
        public List<uint> QuestsToValidId { get; set; }
        public List<uint> QuestsToStartId { get; set; }


        // Constructors
        public GameRolePlayNpcQuestFlag() { }

        public GameRolePlayNpcQuestFlag(List<uint> questsToValidId = null, List<uint> questsToStartId = null)
        {
            QuestsToValidId = questsToValidId;
            QuestsToStartId = questsToStartId;
        }

    }
}
