using System.Linq;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Accounts.InGame.Map.Entities
{
    public class MovableEntity
    {
        // Properties
        public short CellId { get; protected set; }


        #region Updates

        public void Update(GameMapMovementMessage message)
        {
            CellId = (short) message.KeyMovements.Last();
        }

        #endregion
    }
}