using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Storage
{
    public class StorageGetItemAction : ScriptAction
    {
        // Constructor
        public StorageGetItemAction(int gid, uint quantity)
        {
            GID = gid;
            Quantity = quantity;
        }

        // Properties
        // Properties
        public int GID { get; }
        public uint Quantity { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Storage.GetItem(GID, (int) Quantity)) await Task.Delay(1000);

            return ScriptActionResults.DONE;
        }
    }
}