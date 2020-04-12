using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class StartExchangeActionByName : ScriptAction
    {
        //Property
        public string PlayerName { get; private set; }

        // Constructor
        public StartExchangeActionByName(string playername)
        {
            PlayerName = playername;
        }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Exchange.StartExchangeByName(PlayerName))
                return ProcessingResult;

            return DoneResult;
        }
    }
}
