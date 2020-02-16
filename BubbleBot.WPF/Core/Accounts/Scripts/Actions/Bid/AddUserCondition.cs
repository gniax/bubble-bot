using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Bid
{
    public class AddUserCondition : ScriptAction
    {

        // Properties
        public int ItemEffectsId { get; private set; }
        public string ItemCondition { get; private set; }
        public int ItemValue { get; private set; }

        // Constructor
        public AddUserCondition(int itemeffectid, string itemcondition, int itemvalue)
        {
            ItemEffectsId = itemeffectid;
            ItemCondition = itemcondition;
            ItemValue = itemvalue;
        }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Bid.AddBuyItemCondition(ItemEffectsId, ItemCondition, ItemValue))
            {
                await Task.Delay(500);
            }

            return ScriptActionResults.DONE;
        }

    }
}