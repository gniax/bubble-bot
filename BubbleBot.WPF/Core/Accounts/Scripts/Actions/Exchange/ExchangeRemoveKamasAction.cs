using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class ExchangeRemoveKamasAction : ScriptAction
    {
        // Constructor
        public ExchangeRemoveKamasAction(uint qty)
        {
            Quantity = qty;
        }

        // Properties
        public uint Quantity { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Exchange.RemoveKamas(Quantity)) await Task.Delay(2000);

            return ScriptActionResults.DONE;
        }
    }
}