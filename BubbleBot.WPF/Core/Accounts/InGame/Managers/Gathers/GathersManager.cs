using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Managers.Movements;
using BubbleBot.Core.Accounts.InGame.Map;
using BubbleBot.Core.Accounts.InGame.Map.Interactives;
using BubbleBot.Core.Enums;
using BubbleBot.Core.Pathfinding;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Accounts.InGame.Managers.Gathers
{
    public class GathersManager : IClearable, IDisposable
    {
        // Fields
        private Account _account;
        private List<uint> _blackListedElements;
        private InteractiveElementEntry _elementToGather;
        private Pathfinder _pathfinder;
        private bool _stolen;


        // Constructor
        public GathersManager(Account account, MovementsManager movements, MapGame map)
        {
            _account = account;
            _blackListedElements = new List<uint>();
            _pathfinder = new Pathfinder();

            movements.MovementFinished += Movements_MovementFinished;
            map.MapChanged += Map_MapChanged;
            _account.Network.RegisterMessage<InteractiveUsedMessage>(HandleInteractiveUsedMessage);
            _account.Network.RegisterMessage<InteractiveUseEndedMessage>(HandleInteractiveUseEndedMessage);
            _account.Network.RegisterMessage<InteractiveUseErrorMessage>(HandleInteractiveUseErrorMessage);
        }

        public void Clear()
        {
            _elementToGather = null;
            _blackListedElements.Clear();
            _stolen = false;
        }


        // Events
        public event Action<GatherResults> GatherFinished;
        public event Action GatherStarted;


        public bool CanGather(List<int> ressourcesIds)
        {
            return GetUsableElements(ressourcesIds).Count > 0;
        }

        public void CancelGather()
        {
            _elementToGather = null;
        }

        public bool Gather(List<int> ressourcesIds)
        {
            if (_account.IsBusy || _elementToGather != null)
            {
                _account.Logger.LogWarning("GathersManager", $"Is busy ({_account.State}) or is already gathering.");
                return false;
            }

            foreach (var kvp in GetUsableElements(ressourcesIds))
                if (MoveToElement(kvp))
                    return true;

            _account.Logger.LogWarning("GathersManager", "No reachable resource found.");
            return false;
        }

        private Dictionary<InteractiveElementEntry, short> GetUsableElements(List<int> ressourcesIds)
        {
            var usableElements = new Dictionary<InteractiveElementEntry, short>();

            try
            {
                var hasFishingRod = _account.Game.Character.Inventory.HasFishingRod;
                var weaponRange = _account.Game.Character.Inventory.GetWeaponRange();

                foreach (var interactive in _account.Game.Map.Interactives)
                {
                    // The element must be usable
                    if (!interactive.Usable)
                        continue;

                    // Check if the element is blacklisted
                    if (_blackListedElements.Contains(interactive.Id))
                        continue;

                    // Check if this interactive is something we want
                    if (!ressourcesIds.Contains(interactive.ElementTypeId))
                        continue;

                    var statedElement = _account.Game.Map.GetStatedElement((int) interactive.Id);
                    var elementMp = MapPoint.FromCellId(statedElement.CellId);

                    var path = _pathfinder.GetPath(_account.Game.Map.PlayedCharacter.CellId, statedElement.CellId,
                        _account.Game.Map.OccupiedCells, true, true);

                    // If the path is invalid
                    if (path.Count == 0)
                        continue;

                    // Check the distance between where we will be and the element we're intressted in
                    // If we have a fishing rod, we need to compare it with the rod's range, otherwise we'll compare it to 1
                    if (hasFishingRod && MapPoint.FromCellId(path.Last()).DistanceToCell(elementMp) <= weaponRange ||
                        !hasFishingRod && MapPoint.FromCellId(path.Last()).DistanceTo(elementMp) == 1)
                    {
                        // Check if this path ends with a group of monsters
                        if (_account.Game.Map.MonstersGroups.FirstOrDefault(mg => mg.CellId == path.Last()) != null)
                            continue;

                        usableElements.Add(interactive, statedElement.CellId);
                    }
                }
            }
            catch
            {
            }

            return usableElements;
        }

        private bool MoveToElement(KeyValuePair<InteractiveElementEntry, short> element)
        {
            _elementToGather = element.Key;

            // Assuming there is no way statedElem will be null
            switch (_account.Game.Managers.Movements.MoveToCell(element.Value, false, true))
            {
                case MovementRequestResults.MOVED:
                    _account.Logger.LogDebug(LanguageManager.Translate("133"),
                        LanguageManager.Translate("122", element.Value, element.Key.Id));
                    return true;
                case MovementRequestResults.ALREADY_THERE:
                    TryUsingElementToGather();
                    return true;
                default: // FAILED
                    CancelGather();
                    return false;
            }
        }

        private void TryUsingElementToGather()
        {
            if (_stolen)
            {
                _account.Logger.LogInfo(LanguageManager.Translate("133"), LanguageManager.Translate("123"));
                OnGatherFinished(GatherResults.STOLEN);
            }
            else
            {
                _account.Logger.LogDebug(LanguageManager.Translate("133"),
                    LanguageManager.Translate("124", _elementToGather.Id));
                _account.Network.SendMessage(new InteractiveUseRequestMessage(_elementToGather.Id,
                    _elementToGather.EnabledSkills[0].InstanceUID));
            }
        }

        private void Movements_MovementFinished(bool success)
        {
            if (_elementToGather == null)
                return;

            if (success)
                TryUsingElementToGather();
            else
                OnGatherFinished(GatherResults.FAILED);
        }

        private Task HandleInteractiveUsedMessage(Account account, InteractiveUsedMessage message)
        {
            return Task.Run(() =>
            {
                if (_elementToGather == null || _elementToGather.Id != message.ElemId)
                    return;

                // Check if the element has been stolen
                if (message.EntityId != _account.Game.Character.Id)
                {
                    _stolen = true;
                }
                else
                {
                    _account.State = AccountStates.GATHERING;
                    GatherStarted?.Invoke();
                }
            });
        }

        private Task HandleInteractiveUseEndedMessage(Account account, InteractiveUseEndedMessage message)
        {
            return Task.Run(() =>
            {
                _account.State = AccountStates.NONE;
                _account.Logger.LogDebug(LanguageManager.Translate("133"), LanguageManager.Translate("126"));
                OnGatherFinished(GatherResults.GATHERED);
            });
        }

        private Task HandleInteractiveUseErrorMessage(Account account, InteractiveUseErrorMessage message)
        {
            return Task.Run(() =>
            {
                account.Logger.LogWarning(LanguageManager.Translate("133"),
                    LanguageManager.Translate("127", _elementToGather.Id));
                _blackListedElements.Add(_elementToGather.Id);
                OnGatherFinished(GatherResults.BLACKLISTED);
            });
        }

        private void Map_MapChanged()
        {
            _pathfinder.SetMap(_account.Game.Map.Data);
            _blackListedElements.Clear();
        }

        private void OnGatherFinished(GatherResults result)
        {
            _stolen = false;
            _elementToGather = null;
            GatherFinished?.Invoke(result);
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) _pathfinder.Dispose();

                _blackListedElements.Clear();
                _blackListedElements = null;
                _elementToGather = null;
                _pathfinder = null;
                _account = null;

                _disposedValue = true;
            }
        }

        ~GathersManager()
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