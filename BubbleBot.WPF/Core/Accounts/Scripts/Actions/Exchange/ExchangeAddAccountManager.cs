using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class ExchangeAddAccountManager : ScriptAction
    {
        internal override async Task<ScriptActionResults> Process(Account account)
        {
            await Task.Delay(1);
            account.Game.Exchange.AddPlayerAuthorization(account.Game.Character.Id);
            account.Logger.LogInfo("Exchange", LanguageManager.Translate("674"));
            return ScriptActionResults.DONE;
        }
    }
}