using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Bid
{
    public class ExtendBuyItemAction : ScriptAction
    {
        // Constructor
        public ExtendBuyItemAction(uint gid, uint lot, uint maxPrice = 0)
        {
            GID = gid;
            Lot = lot;
            MaxPrice = maxPrice;
        }

        // Properties
        public uint GID { get; }
        public uint Lot { get; }
        public uint MaxPrice { get; }

        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (await account.Game.Bid.ExtendedBuyItemAsync(GID, Lot, MaxPrice)) await Task.Delay(1500);

            return ScriptActionResults.DONE;
        }
    }
}