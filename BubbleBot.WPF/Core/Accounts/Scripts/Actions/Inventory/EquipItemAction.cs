using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Inventory
{
    public class EquipItemAction : ScriptAction
    {
        // Constructor
        public EquipItemAction(int gid)
        {
            GID = gid;
        }

        // Properties
        public int GID { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            var obj = account.Game.Character.Inventory.GetObjectByGID(GID);

            if (obj != null && account.Game.Character.Inventory.EquipObject(obj)) await Task.Delay(500);

            return ScriptActionResults.DONE;
        }
    }
}