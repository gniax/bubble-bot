using System.Collections.Generic;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Managers.Movements;
using BubbleBot.Core.Accounts.InGame.Fights;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Fight
{
    public class ForceFightAction : ScriptAction
    {
        // Constructor
        public ForceFightAction(int minMonsters, int maxMonsters, int minMonstersLevel, int maxMonstersLevel,
            List<int> forbiddenMonsters, List<int> mandatoryMonsters)
        {
            MinMonsters = minMonsters;
            MaxMonsters = maxMonsters;
            MinMonstersLevel = minMonstersLevel;
            MaxMonstersLevel = maxMonstersLevel;
            ForbiddenMonsters = forbiddenMonsters;
            MandatoryMonsters = mandatoryMonsters;
        }

        // Properties
        public int MinMonsters { get; }
        public int MaxMonsters { get; }
        public int MinMonstersLevel { get; }
        public int MaxMonstersLevel { get; }
        public List<int> ForbiddenMonsters { get; }
        public List<int> MandatoryMonsters { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
         //   if (account.HasGroup && !account.IsGroupChief)
           //     return DoneResult;

            if (account.Game.Fight.ForceFight(ForbiddenMonsters, MandatoryMonsters, MinMonsters, MaxMonsters, MinMonstersLevel, MaxMonstersLevel).Result)
                return DoneResult;
            else
                return FailedResult;

        }
    }
}