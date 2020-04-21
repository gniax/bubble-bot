using BubbleBot.Protocol.Types;

namespace BubbleBot.Core.Accounts.InGame.Map.Interactives
{
    public class StatedElementEntry
    {
        // Constructor
        public StatedElementEntry(StatedElement elem)
        {
            Id = elem.ElementId;
            CellId = (short) elem.ElementCellId;
            State = (byte) elem.ElementState;
        }

        // Properties
        public short CellId { get; }
        public uint Id { get; }
        public byte State { get; }
    }
}