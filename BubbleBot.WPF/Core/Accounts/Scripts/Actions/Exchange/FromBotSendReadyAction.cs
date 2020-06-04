using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class FromBotSendReadyAction : ScriptAction
    {
        // Constructor
        public FromBotSendReadyAction(string groupmng, string idmng)
        {
            GroupMng = groupmng;
            IdMng = idmng;
        }

        //Property
        public string GroupMng { get; }
        public string IdMng { get; }
        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return DoneResult;

            foreach (var acc in BubbleBotMain.Instance.ConnectedAccounts)
            {
                if (acc.IsGroupChief && acc.HasGroup)
                {
                    foreach (var member in acc.Group.Members)
                    {
                        if (member.AccountConfig.Nickname == GroupMng && member.AccountConfig.Identifiant == IdMng)
                        {
                            member.Game.Exchange.SendReady();
                        }
                    }
                }

                if (acc.AccountConfig.Nickname == GroupMng && acc.AccountConfig.Identifiant == IdMng)
                {
                    acc.Game.Exchange.SendReady();
                }
            }
            
            return DoneResult;
        }
    }
}