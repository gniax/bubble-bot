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

        public StartExchangeAction(Account source, int playerId)
        {
            Source = source;
            PlayerId = playerId;
        }

        // Properties
        public Account Source { get; }
        public int PlayerId { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (Source != null)
            {
                if (account.HasGroup && !account.IsGroupChief)
                    return DoneResult;

                if (Source.Game.Exchange.StartExchange(PlayerId))
                    return DoneResult;
            }

            if (account.Game.Exchange.StartExchange(PlayerId))
                return DoneResult;

            return DoneResult;
        }
    }
}