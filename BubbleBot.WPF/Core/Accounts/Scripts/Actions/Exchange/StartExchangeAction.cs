using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class StartExchangeAction : ScriptAction
    {
        // Constructor
        public StartExchangeAction(int playerId)
        {
            PlayerId = playerId;
        }

        // Properties
        public int PlayerId { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Exchange.StartExchange(PlayerId))
                return DoneResult;

            return DoneResult;
        }
    }
}