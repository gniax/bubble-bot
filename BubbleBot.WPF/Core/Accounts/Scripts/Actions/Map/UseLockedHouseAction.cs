using BubbleBot.Configurations.Language;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class UseLockedHouseAction : ScriptAction
    {

        // Properties
        public short DoorCellId { get; }
        public string LockCode { get; }


        // Constructor
        public UseLockedHouseAction(short doorCellId, string lockCode)
        {
            DoorCellId = doorCellId;
            LockCode = lockCode;
        }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (!account.Game.Managers.Interactives.UseLockedDoor(DoorCellId, LockCode))
            {
                account.Scripts.StopScript(LanguageManager.Translate("565"));
                return FailedResult;
            }

            return ProcessingResult;
        }

    }
}
