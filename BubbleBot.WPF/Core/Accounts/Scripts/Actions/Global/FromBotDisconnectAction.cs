using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Enums;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class FromBotDisconnectAction : ScriptAction
    {
        // Constructor
        public FromBotDisconnectAction(string groupmng, string idmng)
        {
            GroupMng = groupmng;
            IdMng = idmng;
        }

        // Properties
        public string GroupMng { get; }
        public string IdMng { get; }

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
                            if (member.State == AccountStates.DISCONNECTED)
                                return ScriptActionResults.DONE;

                            await member.Network.Disconnect("Script Action");

                            return ScriptActionResults.DONE;
                        }
                    }
                }
                if (acc.AccountConfig.Nickname == GroupMng && acc.AccountConfig.Identifiant == IdMng)
                {
                    if (acc.State == AccountStates.DISCONNECTED)
                        return ScriptActionResults.DONE;

                    await acc.Network.Disconnect("Script Action");

                    return ScriptActionResults.DONE;
                }
            }
            return ScriptActionResults.DONE;
        }
    }
}