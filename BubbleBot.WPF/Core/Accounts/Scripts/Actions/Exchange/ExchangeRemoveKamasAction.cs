using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class ExchangeRemoveKamasAction : ScriptAction
    {
        // Constructor
        public ExchangeRemoveKamasAction(Account source, uint qty)
        {
            Source = source;
            Quantity = qty;
        }
        public ExchangeRemoveKamasAction(uint qty)
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

                if (Source.Game.Exchange.RemoveKamas(Quantity)) await Task.Delay(2000);

                return ScriptActionResults.DONE;
            }

            if (account.Game.Exchange.RemoveKamas(Quantity)) await Task.Delay(2000);

            return ScriptActionResults.DONE;
        }
    }
}