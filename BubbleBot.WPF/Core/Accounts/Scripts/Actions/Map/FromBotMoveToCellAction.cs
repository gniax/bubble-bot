using System.Threading.Tasks;
using BubbleBot.Core.Accounts.InGame.Managers.Movements;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class FromBotMoveToCellAction : ScriptAction
    {
        // Constructor
        public FromBotMoveToCellAction(string groupmng, string idmng,short cellId)
        {
            GroupMng = groupmng;
            IdMng = idmng;
            CellId = cellId;
        }

        // Properties
        public string GroupMng { get; }
        public string IdMng { get; }
        public short CellId { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return DoneResult;

            switch (account.Game.Managers.Movements.FromBotMoveToCell(GroupMng, IdMng,CellId))
            {
                case MovementRequestResults.MOVED:
                    return DoneResult;
                case MovementRequestResults.PATH_BLOCKED:
                case MovementRequestResults.ALREADY_THERE:
                    return DoneResult;
                default: // FAILED
                    return FailedResult;
            }
        }
    }
}