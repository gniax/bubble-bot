using System;
using System.Collections.Generic;
using System.Linq;
using BubbleBot.Core.Pathfinding;
using BubbleBot.Core.Pathfinding.Fights;
using BubbleBot.Protocol.Data;
using BubbleBot.Utility;

namespace BubbleBot.Core.Accounts.Extensions.Fights.Utility
{
    public class FightsUtility : IDisposable
    {
        // Fields
        private Account _account;


        // Constructor
        public FightsUtility(Account account)
        {
            _account = account;
        }

        #region Spells

        public bool SpellIsHittingAnyEnnemy(short fromCellId, SpellLevels spellLevel)
        {
            foreach (var spellCell in _account.Game.Fight.GetSpellRange(fromCellId, spellLevel))
            foreach (var ennemy in _account.Game.Fight.Ennemies)
                // This ennemy is in range
                if (spellCell == ennemy.CellId)
                    return true;

            return false;
        }

        #endregion


        #region Distances / Cells

        public KeyValuePair<short, MoveNode>? GetNearestOrFarthestEndMoveNode(bool nearest,
            bool basedOnAllMonsters = true)
        {
            KeyValuePair<short, MoveNode>? node = null;
            var totalDistances = -1;
            var distance = -1;

            // Include our current cell
            totalDistances = basedOnAllMonsters
                ? GetTotalDistancesFromEnnemies(_account.Game.Fight.PlayedFighter.CellId)
                : MapPoint.FromCellId(_account.Game.Fight.PlayedFighter.CellId)
                    .DistanceToCell(MapPoint.FromCellId(_account.Game.Fight.GetNearestEnnemy().CellId));

            foreach (var kvp in FightsPathfinder.GetReachableZone(_account.Game.Fight, _account.Game.Map.Data,
                _account.Game.Fight.PlayedFighter.CellId))
            {
                if (!kvp.Value.Reachable)
                    continue;

                var tempTotalDistances = basedOnAllMonsters
                    ? GetTotalDistancesFromEnnemies(kvp.Key)
                    : MapPoint.FromCellId(kvp.Key)
                        .DistanceToCell(MapPoint.FromCellId(_account.Game.Fight.GetNearestEnnemy().CellId));

                if (nearest && tempTotalDistances <= totalDistances || !nearest && tempTotalDistances >= totalDistances)
                {
                    if (nearest)
                    {
                        node = kvp;
                        totalDistances = tempTotalDistances;
                    }
                    // If we need to give the farthest cell, we might aswell give the one that uses the most available MP
                    else if (kvp.Value.Path.Reachable.Count >= distance)
                    {
                        node = kvp;
                        totalDistances = tempTotalDistances;
                        distance = kvp.Value.Path.Reachable.Count;
                    }
                }
            }

            return node;
        }

        public KeyValuePair<short, MoveNode>? GetRandomPossibleNode()
        {
            KeyValuePair<short, MoveNode>? node = null;
            var possibleCells = new List<KeyValuePair<short, MoveNode>>();

            foreach (var kvp in FightsPathfinder.GetReachableZone(_account.Game.Fight, _account.Game.Map.Data,
                _account.Game.Fight.PlayedFighter.CellId))
            {
                if (!kvp.Value.Reachable)
                    continue;

                possibleCells.Add(kvp);
            }

            // Count every member of list and generate random int between 0 and count then return it
            if (possibleCells?.Count > 0)
            {
                var randomIndex = Randomize.GetRandomInt(0, possibleCells.Count);
                node = possibleCells.ElementAt(randomIndex);
            }

            return node;
        }

        public short GetNearestOrFarthestCell(bool nearest, IEnumerable<short> possibleCells)
        {
            short cellId = -1;
            var totalDistances = -1;

            foreach (var cell in possibleCells)
            {
                var tempTotalDistances = GetTotalDistancesFromEnnemies(cell);

                if (cellId == -1 || nearest && tempTotalDistances < totalDistances ||
                    !nearest && tempTotalDistances > totalDistances)
                {
                    cellId = cell;
                    totalDistances = tempTotalDistances;
                }
            }

            return cellId;
        }

        public int GetTotalDistancesFromEnnemies(short fromCellId)
        {
            return _account.Game.Fight.Ennemies.Sum(e =>
                MapPoint.FromCellId(fromCellId).DistanceToCell(MapPoint.FromCellId(e.CellId)) - 1);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _account = null;

                disposedValue = true;
            }
        }

        ~FightsUtility()
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