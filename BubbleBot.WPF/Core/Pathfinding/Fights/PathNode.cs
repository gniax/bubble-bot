namespace BubbleBot.Core.Pathfinding.Fights
{
    public class PathNode
    {
        // Constructor
        public PathNode(short cellId, int mp, int ap, int tackleMp, int tackleAp, int distance)
        {
            CellId = cellId;
            AvailableMp = mp;
            AvailableAp = ap;
            TackleMp = tackleMp;
            TackleAp = tackleAp;
            Distance = distance;
        }

        // Properties
        public short CellId { get; }
        public int AvailableMp { get; }
        public int AvailableAp { get; }
        public int TackleMp { get; }
        public int TackleAp { get; }
        public int Distance { get; }
    }
}