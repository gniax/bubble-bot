using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Enums;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class FromBotConnectAction : ScriptAction
    {
        // Constructor
        public FromBotConnectAction(string groupmng, string idmng, bool rs)
        {
            GroupMng = groupmng;
            IdMng = idmng;
            RestartScript = rs;
        }

        // Properties
        public string GroupMng { get; }
        public string IdMng { get; }
        public bool RestartScript { get; }

        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return ScriptActionResults.DONE;

            foreach (var acc in BubbleBotMain.Instance.ConnectedAccounts)
            {
                if (acc.IsGroupChief && acc.HasGroup)
                {
                    foreach (var member in acc.Group.Members)
                    {
                        if (member.AccountConfig.Nickname == GroupMng && member.AccountConfig.Identifiant == IdMng)
                        {
                            if(member.State != AccountStates.DISCONNECTED)
                                return ScriptActionResults.DONE;

                            if (RestartScript)
                            {
                                member.WaitForRestartScript = true;
                                // member.Scripts.StartScript();
                            }

                            await member.Connect().ConfigureAwait(true);
                            await Task.Delay(1000);



                            return ScriptActionResults.DONE;
                        }
                    }
                }
                if (acc.AccountConfig.Nickname == GroupMng && acc.AccountConfig.Identifiant == IdMng)
                {
                    if (acc.State != AccountStates.DISCONNECTED)
                        return ScriptActionResults.DONE;

                    if (RestartScript)
                    {
                        acc.WaitForRestartScript = true;
                        //acc.Scripts.StartScript();
                    }
                    await acc.Connect().ConfigureAwait(true);
                    await Task.Delay(1000);

                    return ScriptActionResults.DONE;
                }
            }
            return ScriptActionResults.DONE;
        }
    }
}