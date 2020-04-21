using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Bid
{
    public class AddBuyCondition : ScriptAction
    {
        // Constructor
        public AddBuyCondition(uint itemeffectid, string itemcondition, int itemvalue)
        {
            ItemEffectId = itemeffectid;
            ItemCondition = itemcondition;
            ItemValue = itemvalue;
        }

        // Properties
        public uint ItemEffectId { get; }
        public string ItemCondition { get; }
        public int ItemValue { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Bid.AddBuyItemCondition(ItemEffectId, ItemCondition, ItemValue)) await Task.Delay(500);

            return ScriptActionResults.DONE;
        }
    }
}