namespace BubbleBot.Core.Accounts.Scripts.Flags
{
    public class PhenixFlag : IFlag
    {
        // Constructor
        public PhenixFlag(short cellId)
        {
            CellId = cellId;
        }

        // Properties
        public short CellId { get; }
    }
}