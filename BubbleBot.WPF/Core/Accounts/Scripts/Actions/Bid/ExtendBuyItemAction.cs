using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Bid
{
    public class ExtendBuyItemAction : ScriptAction
    {

        // Properties
        public uint GID { get; private set; }
        public uint Lot { get; private set; }

        // Constructor
        public ExtendBuyItemAction(uint gid, uint lot)
        {
            GID = gid;
            Lot = lot;
        }

        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Bid.ExtendedBuyItem(GID, Lot))
            {
                await Task.Delay(1500);
            }

            return ScriptActionResults.DONE;
        }


    }
}
