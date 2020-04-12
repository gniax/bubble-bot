using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Bid
{
    public class ExtendBuyItemAction : ScriptAction
    {

        // Properties
        public uint GID { get; private set; }
        public uint Lot { get; private set; }
        public uint MaxPrice { get; private set; }

        // Constructor
        public ExtendBuyItemAction(uint gid, uint lot, uint maxPrice = 0)
        {
            GID = gid;
            Lot = lot;
            MaxPrice = maxPrice;
        }

        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Bid.ExtendedBuyItem(GID, Lot, MaxPrice))
            {
                await Task.Delay(1500);
            }

            return ScriptActionResults.DONE;
        }


    }
}
