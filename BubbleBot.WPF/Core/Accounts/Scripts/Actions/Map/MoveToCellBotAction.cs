using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Core.Accounts.InGame.Managers.Movements;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class MoveToCellBotAction : ScriptAction
    {
        // Constructor
        public MoveToCellBotAction(Account target, short cellId)
        {
            Target = target;
            CellId = cellId;
        }

        // Properties
        public Account Target { get; }
        public short CellId { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return DoneResult;

            if (account.Game.Managers.Movements.MoveToCellBot(Target, CellId))
                return DoneResult;

            return FailedResult;
        }
    }
}