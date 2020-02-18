using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class ExchangeAddAccountManager : ScriptAction
    {
        internal override async Task<ScriptActionResults> Process(Account account)
        {
            Account.addAutorizedPlayer(account.Game.Character.Id);
            account.Logger.LogInfo("Exchange", "Ajout de l'id à la liste des autorisations.");
            return ScriptActionResults.DONE;
        }

    }
}
