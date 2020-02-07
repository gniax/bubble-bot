using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class SaveZaapAction : ScriptAction
    {

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (!account.Game.Managers.Teleportables.SaveZaap())
            {
                account.Scripts.StopScript(LanguageManager.Translate("538"));
                return FailedResult;
            }

            return ProcessingResult;
        }

    }
}
