using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class ExchangePutKamasAction : ScriptAction
    {
        // Constructor
        public ExchangePutKamasAction(Account source, uint qty)
        {
            Source = source;
            Quantity = qty;
        }
        public ExchangePutKamasAction(uint qty)
        {
            Quantity = qty;
        }

        // Properties
        public Account Source { get; }
        public uint Quantity { get; }

        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (Source != null)
            {
                if (account.HasGroup && !account.IsGroupChief)
                    return ScriptActionResults.DONE;

                if (Source.Game.Exchange.PutKamas(Quantity)) await Task.Delay(2000);

                return ScriptActionResults.DONE;
            }

            if (account.Game.Exchange.PutKamas(Quantity)) await Task.Delay(2000);

            return ScriptActionResults.DONE;
        }
    }
}