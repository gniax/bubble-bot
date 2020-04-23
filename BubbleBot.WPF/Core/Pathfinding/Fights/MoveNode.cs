namespace BubbleBot.Core.Pathfinding.Fights
{
    public class MoveNode
    {
        // Constructor
        public MoveNode(int ap, int mp, short from, bool reachable)
        {
            Ap = ap;
            Mp = mp;
            From = from;
            Reachable = reachable;
        }

        // Properties
        public int Ap { get; }
        public int Mp { get; }
        public short From { get; }
        public bool Reachable { get; }
        public FightPath Path { get; set; }
    }
}