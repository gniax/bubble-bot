using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Storage
{
    public class StorageGetKamasAction : ScriptAction
    {
        // Constructor
        public StorageGetKamasAction(int amount)
        {
            Amount = amount;
        }

        // Properties
        public int Amount { get; }

        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Storage.GetKamas(Amount)) await Task.Delay(1000);

            return ScriptActionResults.DONE;
        }
    }
}