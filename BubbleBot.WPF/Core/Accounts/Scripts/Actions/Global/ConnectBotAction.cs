using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Enums;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class ConnectBotAction : ScriptAction
    {
        // Constructor
        public ConnectBotAction(string account)
        {
            AccountName = account;;
        }

        // Properties
        public string AccountName { get; set; }

        internal override async Task<ScriptActionResults> Process(Account account)
        {
            foreach (var acc in BubbleBotMain.Instance.EveryConnectedAccount())
            {
                if (AccountName != null && AccountName == acc.AccountConfig.Username)
                {
                    if (acc.State != AccountStates.DISCONNECTED && acc.State != AccountStates.BANNED)
                        return ScriptActionResults.DONE;

                    acc.WaitForRestartScript = true;
                    
                    await acc.Connect().ConfigureAwait(true);
                    await Task.Delay(1000);

                    return ScriptActionResults.DONE;
                }
                           
            }
            return ScriptActionResults.DONE;
        }
    }
}