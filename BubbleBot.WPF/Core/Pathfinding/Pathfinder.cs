using System;
using System.Collections.Generic;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Accounts.InGame.Map.Entities;
using BubbleBot.Protocol.Data.Maps;

namespace BubbleBot.Core.Pathfinding
{
    public class Pathfinder : IDisposable
    {
        // Fields
        private const int OCCUPIED_CELL_WEIGHT = 10;
        private const double ELEVATION_TOLERANCE = 11.825;
        private const int WIDTH = 35;
        private const int HEIGHT = 36;
        private short _firstCellZone;

        private CellData[,] _grid;
        private bool _oldMovementSystem;


        // Constructor
        public Pathfinder()
        {
            _grid = new CellData[WIDTH, HEIGHT];
        }


        public void SetMap(Map map)
        {
            _firstCellZone = map.Cells[0].z;
            _oldMovementSystem = true; // TODO: add whether a map uses the old system onto the map data

            for (var i = 0; i < WIDTH; i++)
            for (var j = 0; j < HEIGHT; j++)
            {
                _grid[i, j] = new CellData(i, j);
                var p = MapPoint.FromCoords(i - 1, j - 1);
                UpdateCellPath(p == null ? null : map.Cells[p.CellId], _grid[i, j]);
            }
        }
   
        public List<short> GetPath(short source, short target, List<short> occupiedCells, bool allowDiagonals,
            bool stopNextToTarget)
        {
            int c;
            CellPath candidate;

            //Position de départ et position d'arrivé
            var srcPos = MapPoint.FromCellId(source);
            var dstPos = MapPoint.FromCellId(target);

            var si = srcPos.X + 1;
            var sj = srcPos.Y + 1;

            var srcCell = _grid[si, sj];
            if (srcCell.Zone == -1)
            {
                CellData bestFit = null;
                var bestDist = int.MaxValue;
                var bestFloorDiff = int.MaxValue;

                for (var i = -1; i <= 1; i++)
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    var cell = _grid[si + i, sj + j];
                    if (cell.Zone == -1)
                        continue;

                    var floorDiff = Math.Abs(cell.Floor - srcCell.Floor);
                    var dist = Math.Abs(i) + Math.Abs(j);
                    if (bestFit == null || floorDiff < bestFloorDiff || floorDiff <= bestFloorDiff && dist < bestDist)
                    {
                        bestFit = cell;
                        bestDist = dist;
                        bestFloorDiff = floorDiff;
                    }
                }

                if (bestFit != null)
                    return new List<short> {source, MapPoint.FromCoords(bestFit.I + 1, bestFit.J + 1).CellId};

                throw new Exception($"Player is stuck in '{si}/{sj}.");
            }

            var di = dstPos.X + 1;
            var dj = dstPos.Y + 1;

            MapPoint cellPos;
            foreach (var cellId in occupiedCells)
            {
                cellPos = MapPoint.FromCellId(cellId);
                _grid[cellPos.X + 1, cellPos.Y + 1].Weight += OCCUPIED_CELL_WEIGHT;
            }

            var candidates = new List<CellPath>();
            var selections = new List<CellPath>();

            var distSrcDst = Math.Sqrt(Math.Pow(si - di, 2) + Math.Pow(sj - dj, 2));
            var selection = new CellPath(si, sj, 0, distSrcDst, null);

            CellPath reachingPath = null;
            var closestPath = selection;
            while (selection.I != di || selection.J != dj)
            {
                AddCandidates(selection, di, dj, candidates, allowDiagonals);

                var n = candidates.Count;
                if (n == 0)
                {
                    selection = closestPath;
                    break;
                }

                var minPotentialWeight = double.MaxValue;
                var selectionIndex = 0;
                for (c = 0; c < n; c++)
                {
                    candidate = candidates[c];
                    if (candidate.W + candidate.D < minPotentialWeight)
                    {
                        selection = candidate;
                        minPotentialWeight = candidate.W + candidate.D;
                        selectionIndex = c;
                    }
                }

                selections.Add(selection);
                candidates.RemoveAt(selectionIndex);

                if (selection.D == 0 || stopNextToTarget && selection.D < 1.5)
                {
                    if (reachingPath == null || selection.W < reachingPath.W)
                    {
                        reachingPath = selection;
                        closestPath = selection;

                        var trimmedCandidates = new List<CellPath>();
                        for (c = 0; c < candidates.Count; c++)
                        {
                            candidate = candidates[c];
                            if (candidate.W + candidate.D < reachingPath.W)
                                trimmedCandidates.Add(candidate);
                            else
                                _grid[candidate.I, candidate.J].CandidateRef = null;
                        }

                        candidates = trimmedCandidates;
                    }
                }
                else
                {
                    if (selection.D < closestPath.D) closestPath = selection;
                }
            }

            for (c = 0; c < candidates.Count; c++)
            {
                candidate = candidates[c];
                _grid[candidate.I, candidate.J].CandidateRef = null;
            }

            for (var s = 0; s < selections.Count; s++)
            {
                selection = selections[s];
                _grid[selection.I, selection.J].CandidateRef = null;
            }

            foreach (var cellId in occupiedCells)
            {
                cellPos = MapPoint.FromCellId(cellId);
                _grid[cellPos.X + 1, cellPos.Y + 1].Weight -= OCCUPIED_CELL_WEIGHT;
            }

            var shortestPath = new List<short>();
            while (closestPath != null)
            {
                shortestPath.Insert(0, MapPoint.FromCoords(closestPath.I - 1, closestPath.J - 1).CellId);
                closestPath = closestPath.Path;
            }

            return shortestPath;
        }

        public List<int> CompressPath(List<short> path)
        {
            var compressedPath = new List<int>();

            var prevCellId = path[0];
            var prevDirection = -1;
            int prevX = 0, prevY = 0;

            for (var i = 0; i < path.Count; i++)
            {
                int direction;
                var coord = MapPoint.FromCellId(path[i]);

                if (i == 0)
                {
                    direction = -1;
                }
                else
                {
                    if (coord.Y == prevY)
                    {
                        direction = coord.X > prevX ? 7 : 3;
                    }
                    else if (coord.X == prevX)
                    {
                        direction = coord.Y > prevY ? 1 : 5;
                    }
                    else
                    {
                        if (coord.X > prevX)
                            direction = coord.Y > prevY ? 0 : 6;
                        else
                            direction = coord.Y > prevY ? 2 : 4;
                    }
                }

                if (direction != prevDirection)
                {
                    compressedPath.Add(prevCellId + (direction << 12));
                    prevDirection = direction;
                }

                prevCellId = path[i];
                prevX = coord.X;
                prevY = coord.Y;
            }

            compressedPath.Add(prevCellId + (prevDirection << 12));
            return compressedPath;
        }

        public List<short> NormalizePath(List<short> path)
        {
            if (CheckPath(path)) return path;
            return ExpandPath(path);
        }

        public List<MapPoint> GetAccessibleCells(int i, int j)
        {
            i++;
            j++;
            var c = _grid[i, j];
            var accessibleCells = new List<MapPoint>();

            foreach (var cell in GetAdjacentCells(i, j))
                if (AreCommunicating(c, cell))
                    accessibleCells.Add(MapPoint.FromCoords(cell.I - 1, cell.J - 1));

            return accessibleCells;
        }

        public void UpdateCellPath(short cellId, Cell cell)
        {
            var p = MapPoint.FromCellId(cellId);
            UpdateCellPath(cell, _grid[p.X + 1, p.Y + 1]);
        }

        private List<short> ExpandPath(List<short> path)
        {
            if (path.Count < 2) return path;

            var result = new List<short>();
            result.Add(path[0]);

            var previous = MapPoint.FromCellId(path[0]);
            for (var i = 1; i < path.Count; i++)
            {
                var coord = MapPoint.FromCellId(path[i]);
                int incrX, incrY;

                double c = Math.Abs(coord.X - previous.X);
                double d = Math.Abs(coord.Y - previous.Y);

                if (c == 0 || d == 0)
                {
                    if (c > 1)
                    {
                        incrX = coord.X > previous.X ? 1 : -1;
                        previous.X += incrX;
                        while (previous.X != coord.X)
                        {
                            result.Add(MapPoint.FromCoords(previous.X, previous.Y).CellId);
                            previous.X += incrX;
                        }
                    }

                    if (d > 1)
                    {
                        incrY = coord.Y > previous.Y ? 1 : -1;
                        previous.Y += incrY;
                        while (previous.Y != coord.Y)
                        {
                            result.Add(MapPoint.FromCoords(previous.X, previous.Y).CellId);
                            previous.Y += incrY;
                        }
                    }
                }
                else if (c == d)
                {
                    incrX = coord.X > previous.X ? 1 : -1;
                    incrY = coord.Y > previous.Y ? 1 : -1;

                    while (previous.Y != coord.Y)
                    {
                        result.Add(MapPoint.FromCoords(previous.X, previous.Y).CellId);
                        previous.X += incrX;
                        previous.Y += incrY;
                    }
                }

                previous = coord;
                result.Add(path[i]);
            }

            return result;
        }

        private bool CheckPath(List<short> path)
        {
            if (path.Count < 2)
                return false;

            var previous = MapPoint.FromCellId(path[0]);
            for (var i = 1; i < path.Count; i++)
            {
                var coord = MapPoint.FromCellId(path[i]);
                if (Math.Abs(previous.X - coord.X) > 1) return false;
                if (Math.Abs(previous.Y - coord.Y) > 1) return false;
                previous = coord;
            }

            return true;
        }

        private void UpdateCellPath(Cell cell, CellData cellPath)
        {
            if (cell == null || !cell.IsWalkable(false))
            {
                cellPath.Floor = -1;
                cellPath.Zone = -1;
            }
            else
            {
                cellPath.Floor = cell.f;
                cellPath.Zone = cell.z;
                cellPath.Speed = 1 + cell.s / 10;

                if (cellPath.Zone != _firstCellZone)
                    _oldMovementSystem = false;
            }
        }

        private bool AreCommunicating(CellData c1, CellData c2)
        {
            if (c1.Floor == c2.Floor)
                return true;

            if (c1.Zone == c2.Zone)
                return _oldMovementSystem || c1.Zone != 0 || Math.Abs(c1.Floor - c2.Floor) <= ELEVATION_TOLERANCE;

            return false;
        }

        private bool CanMoveDiagonallyTo(CellData c1, CellData c2, CellData c3, CellData c4)
        {
            return AreCommunicating(c1, c2) && (AreCommunicating(c1, c3) || AreCommunicating(c1, c4));
        }

        private void AddCandidate(CellData c, double weight, int di, int dj, List<CellPath> candidates, CellPath path)
        {
            var distanceToDestination = Math.Sqrt(Math.Pow(di - c.I, 2) + Math.Pow(dj - c.J, 2));
            weight = weight / c.Speed + c.Weight;

            if (c.CandidateRef == null)
            {
                var candidateRef = new CellPath(c.I, c.J, path.W + weight, distanceToDestination, path);
                candidates.Add(candidateRef);
                c.CandidateRef = candidateRef;
            }
            else
            {
                var newWeight = path.W + weight;
                if (newWeight < c.CandidateRef.W)
                {
                    c.CandidateRef.W = newWeight;
                    c.CandidateRef.Path = path;
                }
            }
        }

        private void AddCandidates(CellPath path, int di, int dj, List<CellPath> candidates, bool allowDiagonals)
        {
            var i = path.I;
            var j = path.J;
            var c = _grid[i, j];

            var c01 = _grid[i - 1, j];
            var c10 = _grid[i, j - 1];
            var c12 = _grid[i, j + 1];
            var c21 = _grid[i + 1, j];

            if (AreCommunicating(c, c01)) AddCandidate(c01, 1, di, dj, candidates, path);
            if (AreCommunicating(c, c21)) AddCandidate(c21, 1, di, dj, candidates, path);
            if (AreCommunicating(c, c10)) AddCandidate(c10, 1, di, dj, candidates, path);
            if (AreCommunicating(c, c12)) AddCandidate(c12, 1, di, dj, candidates, path);

            if (allowDiagonals)
            {
                var c00 = _grid[i - 1, j - 1];
                var c02 = _grid[i - 1, j + 1];
                var c20 = _grid[i + 1, j - 1];
                var c22 = _grid[i + 1, j + 1];

                var weightDiagonal = Math.Sqrt(2);

                if (CanMoveDiagonallyTo(c, c00, c01, c10)) AddCandidate(c00, weightDiagonal, di, dj, candidates, path);
                if (CanMoveDiagonallyTo(c, c20, c21, c10)) AddCandidate(c20, weightDiagonal, di, dj, candidates, path);
                if (CanMoveDiagonallyTo(c, c02, c01, c12)) AddCandidate(c02, weightDiagonal, di, dj, candidates, path);
                if (CanMoveDiagonallyTo(c, c22, c21, c12)) AddCandidate(c22, weightDiagonal, di, dj, candidates, path);
            }
        }

        private CellData[] GetAdjacentCells(int i, int j)
        {
            return new CellData[4]
            {
                _grid[i - 1, j],
                _grid[i, j - 1],
                _grid[i, j + 1],
                _grid[i + 1, j]
            };
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                _grid = null;

                disposedValue = true;
            }
        }

        ~Pathfinder()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}