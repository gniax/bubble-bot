using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Inventory
{
    public class UnEquipItemAction : ScriptAction
    {
        // Constructor
        public UnEquipItemAction(int gid)
        {
            GID = gid;
        }

        // Properties
        public int GID { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            var obj = account.Game.Character.Inventory.GetObjectByGID(GID);

            if (obj != null && account.Game.Character.Inventory.UnEquipObject(obj)) await Task.Delay(500);

            return ScriptActionResults.DONE;
        }
    }
}