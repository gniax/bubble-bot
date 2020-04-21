using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Inventory
{
    public class UseItemAction : ScriptAction
    {
        // Constructor
        public UseItemAction(int gid, uint quantity)
        {
            GID = gid;
            Quantity = quantity;
        }

        // Properties
        public int GID { get; }
        public uint Quantity { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            var obj = account.Game.Character.Inventory.GetObjectByGID(GID);

            if (obj != null)
            {
                account.Game.Character.Inventory.UseObject(obj, Quantity);
                await Task.Delay(1);
            }

            return ScriptActionResults.DONE;
        }
    }
}