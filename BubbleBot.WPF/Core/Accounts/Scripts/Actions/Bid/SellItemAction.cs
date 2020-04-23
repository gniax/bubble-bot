using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Bid
{
    public class SellItemAction : ScriptAction
    {
        // Constructor
        public SellItemAction(uint gid, uint lot, uint price)
        {
            GID = gid;
            Lot = lot;
            Price = price;
        }

        // Properties
        public uint GID { get; }
        public uint Lot { get; }
        public uint Price { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Bid.SellItem(GID, Lot, Price)) await Task.Delay(1500);

            return ScriptActionResults.DONE;
        }
    }
}