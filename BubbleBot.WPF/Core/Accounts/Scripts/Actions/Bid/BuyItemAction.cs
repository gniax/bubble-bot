using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Bid
{
    public class BuyItemAction : ScriptAction
    {
        // Constructor
        public BuyItemAction(uint gid, uint lot)
        {
            GID = gid;
            Lot = lot;
        }

        // Properties
        public uint GID { get; }
        public uint Lot { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Bid.BuyItem(GID, Lot)) await Task.Delay(1500);

            return ScriptActionResults.DONE;
        }
    }
}