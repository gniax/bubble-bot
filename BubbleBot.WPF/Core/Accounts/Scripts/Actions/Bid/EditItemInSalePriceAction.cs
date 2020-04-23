using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Bid
{
    public class EditItemInSalePriceAction : ScriptAction
    {
        // Constructor
        public EditItemInSalePriceAction(uint uid, uint newPrice)
        {
            UID = uid;
            NewPrice = newPrice;
        }

        // Properties
        public uint UID { get; }
        public uint NewPrice { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Bid.EditItemInSalePrice(UID, NewPrice)) await Task.Delay(3000);

            return ScriptActionResults.DONE;
        }
    }
}