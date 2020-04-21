using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Storage
{
    public class StoragePutItemAction : ScriptAction
    {
        // Constructor
        public StoragePutItemAction(int gid, uint quantity)
        {
            GID = gid;
            Quantity = quantity;
        }

        // Properties
        public int GID { get; }
        public uint Quantity { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Storage.PutItem(GID, (int) Quantity)) await Task.Delay(1000);

            return ScriptActionResults.DONE;
        }
    }
}