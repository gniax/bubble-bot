using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class ExchangePutItemAction : ScriptAction
    {
        // Constructor
        public ExchangePutItemAction(Account source, int gid, uint qty)
        {
            Source = source;
            GID = gid;
            Quantity = qty;
        }
        public ExchangePutItemAction(int gid, uint qty)
        {
            GID = gid;
            Quantity = qty;
        }

        // Properties
        public Account Source { get; }
        public int GID { get; }
        public uint Quantity { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (Source != null)
            {
                if (account.HasGroup && !account.IsGroupChief)
                    return ScriptActionResults.DONE; 

                if (Source.Game.Exchange.PutItem(GID, Quantity)) await Task.Delay(2000);
                return ScriptActionResults.DONE;
            }

            if (account.Game.Exchange.PutItem(GID, Quantity)) await Task.Delay(2000);

            return ScriptActionResults.DONE;
        }
    }
}