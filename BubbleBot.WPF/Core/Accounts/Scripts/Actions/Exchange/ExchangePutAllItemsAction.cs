using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class ExchangePutAllItemsAction : ScriptAction
    {
        public ExchangePutAllItemsAction(Account source)
        {
            Source = source;
        }

        public ExchangePutAllItemsAction() { }
        // Properties
        public Account Source { get; }
        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (Source != null)
            {
                if (account.HasGroup && !account.IsGroupChief)
                    return ScriptActionResults.DONE;

                if (await Source.Game.Exchange.PutAllItems()) await Task.Delay(2000);

                return ScriptActionResults.DONE;
            }

            if (await account.Game.Exchange.PutAllItems()) await Task.Delay(2000);

            return ScriptActionResults.DONE;
        }
    }
}