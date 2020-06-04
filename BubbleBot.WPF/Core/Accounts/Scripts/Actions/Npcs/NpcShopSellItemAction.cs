using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Npcs
{
    public class NpcShopSellItemAction : ScriptAction
    {
        // Constructor
        public NpcShopSellItemAction(uint gid, int quantity)
        {
            ItemGid = gid;
            ItemQuantity = quantity;
        }

        // Properties
        public uint ItemGid { get; }
        public int ItemQuantity { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {

            if (account.Game.Npcs.NpcShopSellItem(ItemGid, ItemQuantity))
                return ProcessingResult;

            return DoneResult;
        }
    }
}