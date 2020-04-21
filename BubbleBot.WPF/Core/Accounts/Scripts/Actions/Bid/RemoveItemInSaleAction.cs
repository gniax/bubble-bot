using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Bid
{
    public class RemoveItemInSaleAction : ScriptAction
    {
        // Constructor
        public RemoveItemInSaleAction(uint uid)
        {
            UID = uid;
        }

        // Properties
        public uint UID { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Bid.RemoveItemInSale(UID)) await Task.Delay(1500);

            return ScriptActionResults.DONE;
        }
    }
}