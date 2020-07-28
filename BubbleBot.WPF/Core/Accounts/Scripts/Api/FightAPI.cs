using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Core.Accounts.Scripts.Actions.Fight;
using BubbleBot.Core.Accounts.InGame.Fights;
using MoonSharp.Interpreter;

namespace BubbleBot.Core.Accounts.Scripts.Api
{
    [MoonSharpUserData]
    [Obfuscation(Exclude = false, Feature = "-rename", ApplyToMembers = true)]
    public class FightAPI : IDisposable
    {
        // Fields
        public Account _account;


        // Constructor
        public FightAPI(Account account)
        {
            _account = account;
        }


        public bool CanFight(List<int> forbiddenMonsters = null, List<int> mandatoryMonsters = null,
            int minMonsters = 1, int maxMonsters = 8, int minMonstersLevel = 1, int maxMonstersLevel = 1000)
        {
            return _account.Game.Map.CanFight(minMonsters, maxMonsters, minMonstersLevel, maxMonstersLevel,
                forbiddenMonsters, mandatoryMonsters);
        }

        public bool Fight(List<int> forbiddenMonsters = null, List<int> mandatoryMonsters = null, int minMonsters = 1,
            int maxMonsters = 8, int minMonstersLevel = 1, int maxMonstersLevel = 1000)
        {
            if (CanFight(forbiddenMonsters, mandatoryMonsters, minMonsters, maxMonsters, minMonstersLevel,
                maxMonstersLevel))
            {
                _account.Scripts.ActionsManager.EnqueueAction(
                    new FightAction(minMonsters, maxMonsters, minMonstersLevel, maxMonstersLevel, forbiddenMonsters,
                        mandatoryMonsters), true);
                return true;
            }

            return false;
        }

        public uint FightsCount()
        {
            return _account.Statistics.FightsCount;
        }

        public bool ForceFight(List<int> forbiddenMonsters = null, List<int> mandatoryMonsters = null, int minMonsters = 1,
    int maxMonsters = 8, int minMonstersLevel = 1, int maxMonstersLevel = 1000)
        {
            _account.Scripts.ActionsManager.EnqueueAction( new ForceFightAction(minMonsters, maxMonsters, minMonstersLevel, maxMonstersLevel, forbiddenMonsters, mandatoryMonsters), true);

            return true;
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _account = null;


                disposedValue = true;
            }
        }

        ~FightAPI()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}