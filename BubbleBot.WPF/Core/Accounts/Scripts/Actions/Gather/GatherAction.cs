using System.Collections.Generic;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Gather
{
    public class GatherAction : ScriptAction
    {
        // Constructor
        public GatherAction(List<int> elements)
        {
            Elements = elements;
        }

        // Properties
        public List<int> Elements { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            // In case there is no elements to gather in this map, just pass
            if (account.Game.Managers.Gathers.CanGather(Elements))
            {
                if (!account.Game.Managers.Gathers.Gather(Elements))
                {
                    account.Scripts.StopScript(LanguageManager.Translate("169"));
                    return FailedResult;
                }

                return ProcessingResult;
            }

            return DoneResult;
        }
    }
}