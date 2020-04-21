using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class ExchangePutItemAction : ScriptAction
    {
        // Constructor
        public ExchangePutItemAction(int gid, uint qty)
        {
            GID = gid;
            Quantity = qty;
        }

        // Properties
        public int GID { get; }
        public uint Quantity { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Exchange.PutItem(GID, Quantity)) await Task.Delay(2000);

            return ScriptActionResults.DONE;
        }
    }
}