using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class StartExchangeGroupByNameAction : ScriptAction
    {
        // Constructor
        public StartExchangeGroupByNameAction(string playername)
        {
            PlayerName = playername;
        }

        //Property
        public string PlayerName { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if(!account.IsGroupChief)
                return DoneResult;
            
            if (account.Game.Exchange.StartExchangeGroupByName(PlayerName).Result)
                return DoneResult;

            return DoneResult;
        }
    }
}