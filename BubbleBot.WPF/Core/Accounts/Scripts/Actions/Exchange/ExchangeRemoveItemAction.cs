using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class ExchangeRemoveItemAction : ScriptAction
    {
        // Constructor
        public ExchangeRemoveItemAction(int gid, uint qty)
        {
            GID = gid;
            Quantity = qty;
        }

        // Properties
        public int GID { get; }
        public uint Quantity { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Exchange.RemoveItem(GID, Quantity)) await Task.Delay(2000);

            return ScriptActionResults.DONE;
        }
    }
}