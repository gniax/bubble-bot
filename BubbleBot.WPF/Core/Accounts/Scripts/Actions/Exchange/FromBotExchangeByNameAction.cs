using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class FromBotExchangeByNameAction : ScriptAction
    {
        // Constructor
        public FromBotExchangeByNameAction(string groupmng, string idmng, string playername)
        {
            GroupMng = groupmng;
            IdMng = idmng;
            PlayerName = playername;
        }

        //Property
        public string GroupMng { get; }
        public string IdMng { get; }
        public string PlayerName { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if(account.HasGroup && !account.IsGroupChief)
                return DoneResult;

            if (account.Game.Exchange.FromBotExchangeByName(GroupMng,IdMng,PlayerName).Result)
                return DoneResult;

            return DoneResult;
        }
    }
}