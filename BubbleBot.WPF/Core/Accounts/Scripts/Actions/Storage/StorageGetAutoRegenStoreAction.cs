using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Storage
{
    public class StorageGetAutoRegenStoreAction : ScriptAction
    {
        // Constructor
        public StorageGetAutoRegenStoreAction(List<int> items, int store)
        {
            Items = items;
            Store = store;
        }

        // Properties
        public List<int> Items { get; }
        public int Store { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            var store = Store;

            for (var i = 0; i < Items.Count && store > 0; i++)
            {
                // we'll have to get the items manually instead of using Storage.GetITem
                var obj = account.Game.Storage.Objects.FirstOrDefault(o => o.GID == Items[i]);

                if (obj == null)
                    continue;

                // Get the quantity we can actually take
                var validQty = (int) Math.Min(store, obj.Quantity);

                if (account.Game.Storage.GetItem(Items[i], validQty))
                {
                    store -= validQty;
                    await Task.Delay(800);
                }
            }

            if (store > 0)
                account.Logger.LogWarning(LanguageManager.Translate("165"), LanguageManager.Translate("181"));

            return ScriptActionResults.DONE;
        }
    }
}