using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Map;
using BubbleBot.Core.Accounts.InGame.Map.Entities;
using BubbleBot.Core.Enums;
using BubbleBot.Core.Pathfinding;
using BubbleBot.Core.Pathfinding.Fights;
using BubbleBot.Protocol.Enums;
using BubbleBot.Protocol.Messages;
using BubbleBot.Utility;

namespace BubbleBot.Core.Accounts.InGame.Managers.Movements
{
    public class MovementsManager : IClearable, IDisposable
    {
        // Fields
        private Account _account;
        private List<short> _currentPath;
        private int _maxretries;
        private int _neighbourMapId;
        private Pathfinder _pathFinder;
        private int _retries;


        // Constructor
        public MovementsManager(Account account, MapGame map)
        {
            _account = account;
            _pathFinder = new Pathfinder();

            map.MapChanged += Map_MapChanged;
        }

        public void Clear()
        {
            _currentPath = null;
            _neighbourMapId = 0;
            _retries = 0;
            _maxretries = 0;
        }


        // Events
        public event Action<bool> MovementFinished;

        public bool CanChangeMap(short cellId, MapChangeDirections direction)
        {
            switch (direction)
            {
                case MapChangeDirections.LEFT:
                    return (_account.Game.Map.Data.Cells[cellId].c & (int) direction) > 0 && cellId % 14 == 0 &&
                           cellId != 546;
                case MapChangeDirections.RIGHT:
                    return (_account.Game.Map.Data.Cells[cellId].c & (int) direction) > 0 && cellId % 14 == 13 &&
                           cellId != 13;
                case MapChangeDirections.TOP:
                    return (_account.Game.Map.Data.Cells[cellId].c & (int) direction) > 0 && cellId < 27;
                default: // BOTTOM
                    return (_account.Game.Map.Data.Cells[cellId].c & (int) direction) > 0 && cellId > 532;
            }
        }

        public bool ChangeMap(MapChangeDirections direction)
        {
            if (_account.IsBusy || _neighbourMapId != 0)
            {
                _account.Logger.LogWarning("MovementsManager",
                    $"Is busy ({_account.State}) or is already changing the map.");
                return false;
            }

            // TODO: Maybe make this not sot random (pick the closest cell to change map)
            var changeMapCells = GetChangeMapCells(direction);

            while (changeMapCells.Count > 0)
            {
                var cellId = changeMapCells[Randomize.GetRandomInt(0, changeMapCells.Count)];

                // Ignore this cell if a group of monsters is on it
                if (_account.Game.Map.MonstersGroups.FirstOrDefault(mg => mg.CellId == cellId) != null)
                    continue;


                var neighbourMapId = GetNeighbourMapId(direction);

                if (neighbourMapId <= 0)
                {
                    _account.Logger.LogWarning("MovementsManager", "Invalid Neighbour MapId.");
                    return false;
                }

                _neighbourMapId = (int) neighbourMapId;

                // Only return true so that if one cell fails, we try the others
                if (MoveToChangeMap(cellId))
                    return true;

                changeMapCells.Remove(cellId);
            }

            _account.Logger.LogWarning("MovementsManager", "No change map cells found.");
            return false;
        }

        public bool ChangeMap(MapChangeDirections direction, short cellId)
        {
            if (_account.IsBusy || _neighbourMapId != 0)
                return false;

            if (!CanChangeMap(cellId, direction))
                return false;

            var neighbourMapId = GetNeighbourMapId(direction);

            if (neighbourMapId <= 0)
            {
                _account.Logger.LogWarning("MovementsManager", "Invalid Neighbour MapId.");
                return false;
            }

            _neighbourMapId = (int) neighbourMapId;

            return MoveToChangeMap(cellId);
        }

        public MovementRequestResults MoveToCell(short cellId, bool stopNearTarget = false)
        {
            if (cellId < 0 || cellId > 560)
            {
                _account.Logger.LogWarning("MovementsManager", "Invalid cellId.");
                return MovementRequestResults.FAILED;
            }

            if (_account.IsBusy || _currentPath != null)
            {
                _account.Logger.LogWarning("MovementsManager",
                    $"IsBusy: {_account.IsBusy}, PathNotNull: {_currentPath != null}.");
                return MovementRequestResults.FAILED;
            }

            if (cellId == _account.Game.Map.PlayedCharacter.CellId)
                return MovementRequestResults.ALREADY_THERE;

            List<short> aggressiveMonstersGroupCell = new List<short>();
            if (_account.Configuration.AntiAggro)
            {
                foreach (var group in _account.Game.Map.MonstersGroups)
                {
                    List<MonsterEntry> Monsters = new List<MonsterEntry>();
                    if (group.Followers?.Count > 0)
                        Monsters.AddRange(group.Followers);

                    Monsters.Add(group.Leader);
                    if (!Monsters.TrueForAll(i => !AggressiveMonstersEnumFinder.Exists(i.GenericId)))
                    {
                        aggressiveMonstersGroupCell.Add(group.CellId);
                        foreach (var adjCell1 in MapPoint.GetNeighbourCells(group.CellId, false))
                        {
                            if (adjCell1?.CellId != null && adjCell1.CellId >= 0 && adjCell1.CellId <= 560 && !aggressiveMonstersGroupCell.Contains(adjCell1.CellId))
                                aggressiveMonstersGroupCell.Add(adjCell1.CellId);

                            if (adjCell1?.CellId != null)
                            {
                                foreach (var adjCell2 in MapPoint.GetNeighbourCells(adjCell1.CellId, false))
                                {
                                    if (adjCell2?.CellId != null && adjCell2.CellId >= 0 && adjCell2.CellId <= 560 && !aggressiveMonstersGroupCell.Contains(adjCell2.CellId))
                                        aggressiveMonstersGroupCell.Add(adjCell2.CellId);
                                }
                            }
                        }
                    }
                }
            }

            var forbiddenCells = _account.Game.Map.OccupiedCells;

            if (aggressiveMonstersGroupCell?.Count > 0)
                forbiddenCells.AddRange(aggressiveMonstersGroupCell);

            var tempPath = _pathFinder.GetPath(_account.Game.Map.PlayedCharacter.CellId, cellId,
                forbiddenCells, true, stopNearTarget);

            if (tempPath.Count == 0)
            {
                _account.Logger.LogWarning("MovementsManager", "Empty path.");
                return MovementRequestResults.FAILED;
            }

            // StopNearTarget=false case, the path is not complete
            if (!stopNearTarget && tempPath.Last() != cellId)
                return MovementRequestResults.PATH_BLOCKED;

            // StopNearTarget=true case, in case we can't move anywhere near the target cell
            if (stopNearTarget && tempPath.Count == 1 && tempPath[0] == _account.Game.Map.PlayedCharacter.CellId)
                return MovementRequestResults.ALREADY_THERE;

            // StopNearTarget=true case, the character is already next to the target
            if (stopNearTarget && tempPath.Count == 2 && tempPath[0] == _account.Game.Map.PlayedCharacter.CellId && tempPath[1] == cellId)
                return MovementRequestResults.ALREADY_THERE;

            _currentPath = tempPath;
            SendMoveMessage();
            _retries = 0;
            _maxretries = 0;
            return MovementRequestResults.MOVED;
        }

        public async Task MoveToCellInFight(KeyValuePair<short, MoveNode>? node)
        {
            if (_account.State != AccountStates.FIGHTING)
                return;

            if (node == null || node.Value.Value.Path.Reachable.Count == 0)
                return;

            if (node.Value.Key == _account.Game.Fight.PlayedFighter.CellId)
                return;

            // Insert the current cellId
            node.Value.Value.Path.Reachable.Insert(0, _account.Game.Fight.PlayedFighter.CellId);

            if (_account.Extensions.Fights.PlayerCellChangeHistory == null)
                _account.Extensions.Fights.PlayerCellChangeHistory = new List<short>();

            _account.Extensions.Fights.PlayerCellChangeHistory?.Add(node.Value.Key);

            await _account.Network.SendMessageAsync(new GameMapMovementRequestMessage((uint) _account.Game.Map.Id,
                _pathFinder.CompressPath(node.Value.Value.Path.Reachable)));
        }

        private bool MoveToChangeMap(short cellId)
        {
            switch (MoveToCell(cellId))
            {
                case MovementRequestResults.MOVED:
                    _account.Logger.LogDebug("", $"{_account.Game.Map.CurrentPosition} Moving to change map.");
                    return true;
                case MovementRequestResults.ALREADY_THERE:
                    _account.Network.SendMessage(new ChangeMapMessage((uint) _neighbourMapId));
                    _neighbourMapId = 0;
                    return true;
                default: // FAILED or PATH_BLOCKED
                    _account.Logger.LogWarning("MovementsManager", $"Path to {cellId} failed or is blocked.");
                    _neighbourMapId = 0;
                    return false;
            }
        }

        private List<short> GetChangeMapCells(MapChangeDirections direction)
        {
            return Enumerable.Range(0, 560).Select(c => (short) c).Where(c => CanChangeMap(c, direction)).ToList();
        }

        private void SendMoveMessage()
        {
            var compressedPath = _pathFinder.CompressPath(_currentPath);
            _account.Network.SendMessage(
                new GameMapMovementRequestMessage((uint) _account.Game.Map.Id, compressedPath));
            _account.Logger.LogDebug("MovementsManager",
                $"Path: {_currentPath.Select(c => c.ToString()).Aggregate((c, n) => $"{c}, {n}")}");
        }

        public void Cancel()
        {
            Clear();
        }

        private long GetNeighbourMapId(MapChangeDirections direction)
        {
            switch (direction)
            {
                case MapChangeDirections.BOTTOM:
                    return _account.Game.Map.Data.BottomNeighbourId;
                case MapChangeDirections.LEFT:
                    return _account.Game.Map.Data.LeftNeighbourId;
                case MapChangeDirections.RIGHT:
                    return _account.Game.Map.Data.RightNeighbourId;
                case MapChangeDirections.TOP:
                    return _account.Game.Map.Data.TopNeighbourId;
                default:
                    return 0;
            }
        }

        private void OnMovementFinished(bool success)
        {
            _currentPath = null;
            _neighbourMapId = 0;
            MovementFinished?.Invoke(success);
        }

        private void Map_MapChanged()
        {
            _pathFinder.SetMap(_account.Game.Map.Data);
        }

        #region Updates

        public async Task Update(GameMapNoMovementMessage message)
        {
            if (_currentPath == null)
                return;

            if (!_account.Scripts.Running)
            {
                Clear();
                return;
            }

            _retries++;
            _maxretries++;
            if (_maxretries > 9)
            {
                if (_account.State != AccountStates.RECAPTCHA) _account.State = AccountStates.NONE;

                _account.Logger.LogError("MovementsManager", LanguageManager.Translate("631"));
                await _account.Network.Disconnect("CLIENT_CLOSING", true);
                return;
            }

            if (_retries > 3)
            {
                if (_account.State != AccountStates.RECAPTCHA) _account.State = AccountStates.NONE;

                _account.Logger.LogError("MovementsManager", LanguageManager.Translate("55"));
                return;
            }

            _account.Logger.LogWarning("MovementsManager", LanguageManager.Translate("555"));
            await Task.Delay(5000);
            await _account.Network.SendMessageAsync(new MapInformationsRequestMessage((uint) _account.Game.Map.Id))
                .ConfigureAwait(false);

            // In case one of these happen while we were waiting
            if (_currentPath == null)
                return;

            if (!_account.Scripts.Running)
            {
                Clear();
                return;
            }

            SendMoveMessage();
        }

        public Task Update(GameMapMovementMessage message)
        {
            return Task.Run(async () =>
            {
                if (_currentPath != null && message.ActorId == _account.Game.Character.Id &&
                    message.KeyMovements[0] == _currentPath[0] &&
                    _currentPath.Contains((short) message.KeyMovements.Last()))
                {
                    // TODO: Not sure if this is the best way to handle the account's state,
                    //       Also not sure if this is the best way to handle map changements
                    _account.State = AccountStates.MOVING;

                    if (!_account.Configuration.SpeedHack)
                        await Task.Delay(PathDuration.Calculate(_currentPath) + 200);

                    // In case the account was disconnected/disposed
                    if (_account == null || _account.State == AccountStates.DISCONNECTED)
                        return;

                    await _account.Network.SendMessageAsync(new GameMapMovementConfirmMessage());
                    _account.State = AccountStates.NONE;

                    if (_neighbourMapId == 0)
                    {
                        OnMovementFinished(true);
                    }
                    else
                    {
                        _currentPath = null; // Needs to be cleared

                        // Handle the map change request
                        if (_neighbourMapId != 0)
                        {
                            await _account.Network.SendMessageAsync(new ChangeMapMessage((uint) _neighbourMapId));
                            _neighbourMapId = 0;
                        }

                        ;
                    }
                }
            });
        }

        #endregion

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) _pathFinder.Dispose();

                _currentPath?.Clear();
                _currentPath = null;
                _pathFinder = null;
                _account = null;

                _disposedValue = true;
            }
        }

        ~MovementsManager()
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