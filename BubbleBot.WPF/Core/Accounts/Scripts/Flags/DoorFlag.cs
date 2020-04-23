namespace BubbleBot.Core.Accounts.Scripts.Flags
{
    public class DoorFlag : IFlag
    {
        // Constructor
        public DoorFlag(short cellId)
        {
            CellId = cellId;
        }

        // Properties
        public short CellId { get; }
    }
}