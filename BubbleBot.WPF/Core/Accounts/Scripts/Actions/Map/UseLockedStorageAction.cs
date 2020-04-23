using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class UseLockedStorageAction : ScriptAction
    {
        // Constructor
        public UseLockedStorageAction(short elementCellId, string lockCode)
        {
            ElementCellId = elementCellId;
            LockCode = lockCode;
        }

        // Properties
        public short ElementCellId { get; }
        public string LockCode { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (!account.Game.Managers.Interactives.UseLockedStorage(ElementCellId, LockCode))
            {
                account.Scripts.StopScript(LanguageManager.Translate("565"));
                return FailedResult;
            }

            return ProcessingResult;
        }
    }
}