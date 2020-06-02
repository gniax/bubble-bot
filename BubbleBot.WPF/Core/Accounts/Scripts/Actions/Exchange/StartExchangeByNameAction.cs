using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class StartExchangeByNameAction : ScriptAction
    {
        // Constructor
        public StartExchangeByNameAction(string playername)
        {
            PlayerName = playername;
        }

        //Property
        public string PlayerName { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Exchange.StartExchangeByName(PlayerName))
                return ProcessingResult;

            return DoneResult;
        }
    }
}