using System.Collections.Generic;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Managers.Movements;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Fight
{
    public class FightAction : ScriptAction
    {
        // Constructor
        public FightAction(int minMonsters, int maxMonsters, int minMonstersLevel, int maxMonstersLevel,
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
            var availableGroups = account.Game.Map.GetMonstersGroup(MinMonsters, MaxMonsters, MinMonstersLevel,
                MaxMonstersLevel, ForbiddenMonsters, MandatoryMonsters);

            if (availableGroups.Count <= 0)
                return DoneResult;

            for (var i = 0; i < availableGroups.Count; i++)
            {
                if (account.Game.Map.BlacklistedMonsters.Contains(availableGroups[i].Id))
                    continue;

                switch (account.Game.Managers.Movements.MoveToCell(availableGroups[i].CellId))
                {
                    case MovementRequestResults.MOVED:
                        account.Scripts.ActionsManager.MonstersGroupToAttack = availableGroups[i].Id;
                        account.Logger.LogDebug(LanguageManager.Translate("165"),
                            LanguageManager.Translate("166", availableGroups[i].CellId,
                                availableGroups[i].MonstersCount, availableGroups[i].TotalLevel));
                        return ProcessingResult;
                    case MovementRequestResults.ALREADY_THERE:
                    case MovementRequestResults.PATH_BLOCKED:
                        account.Logger.LogWarning(LanguageManager.Translate("165"), LanguageManager.Translate("167"));
                        account.Game.Map.BlacklistedMonsters.Add(availableGroups[i].Id);
                        continue;
                    default: // FAILED
                        account.Scripts.StopScript(LanguageManager.Translate("168"));
                        return FailedResult;
                }
            }

            return DoneResult;
        }
    }
}