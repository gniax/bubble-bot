using System.Threading.Tasks;
using BubbleBot.Core.Accounts.InGame.Managers.Movements;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class MoveToCellAction : ScriptAction
    {
        // Constructor
        public MoveToCellAction(short cellId)
        {
            CellId = cellId;
        }

        // Properties
        public short CellId { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            switch (account.Game.Managers.Movements.MoveToCell(CellId))
            {
                case MovementRequestResults.MOVED:
                    return ProcessingResult;
                case MovementRequestResults.PATH_BLOCKED:
                case MovementRequestResults.ALREADY_THERE:
                    return DoneResult;
                default: // FAILED
                    return FailedResult;
            }
        }
    }
}