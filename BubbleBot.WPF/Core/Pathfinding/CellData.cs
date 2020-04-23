namespace BubbleBot.Core.Pathfinding
{
    public class CellData
    {
        // Constructor
        internal CellData(int i, int j)
        {
            I = i;
            J = j;
        }

        // Properties
        public int I { get; }
        public int J { get; }
        public short Floor { get; internal set; }
        public short Zone { get; internal set; }
        public double Speed { get; internal set; }
        public double Weight { get; internal set; }
        public CellPath CandidateRef { get; internal set; }
    }
}