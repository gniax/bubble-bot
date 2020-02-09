using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Manager
{
    public class GetPlayerLevel : ScriptAction
    {
        // Properties
        public int GID { get; private set; }
        public uint Quantity { get; private set; }


        // Constructor
        public GetPlayerLevel(int gid, uint quantity)
        {
            GID = gid;
            Quantity = quantity;
        }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            var obj = account.Game.Character.Inventory.GetObjectByGID(GID);

            if (obj != null)
            {
                account.Game.Character.Inventory.DropObject(obj, Quantity);
                await Task.Delay(500);
            }

            return ScriptActionResults.DONE;
        }

    }
}
