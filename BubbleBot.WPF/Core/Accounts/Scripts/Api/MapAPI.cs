using System;
using System.Linq;
using System.Reflection;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Managers.Teleportables;
using BubbleBot.Core.Accounts.Scripts.Actions.Global;
using BubbleBot.Core.Accounts.Scripts.Actions.Map;
using MoonSharp.Interpreter;

namespace BubbleBot.Core.Accounts.Scripts.Api
{
    [MoonSharpUserData]
    [Obfuscation(Exclude = false, Feature = "-rename", ApplyToMembers = true)]
    public class MapAPI : IDisposable
    {
        // Fields
        public Account _account;


        // Constructor
        public MapAPI(Account account)
        {
            _account = account;
        }

        #region Basic API
        public bool ChangeMap(string where)
        {
            if (_account.IsBusy)
                return false;

            if (!ChangeMapAction.TryParse(where, out var action))
            {
                _account.Logger.LogWarning(LanguageManager.Translate("182"), LanguageManager.Translate("183", where));
                return false;
            }

            _account.Scripts.ActionsManager.EnqueueAction(action, true);
            return true;
        }

        public bool MoveToCell(short cellId)
        {
            if (cellId < 0 || cellId > 559)
                return false;

            if (!_account.Game.Map.Data.Cells[cellId].IsWalkable(false) ||
                _account.Game.Map.Data.Cells[cellId].IsObstacle())
                return false;

            _account.Scripts.ActionsManager.EnqueueAction(new MoveToCellAction(cellId), true);
            return true;
        }

        public bool FromBotMoveToCell(string groupMng, string idMng,short cellId)
        {
            if (cellId < 0 || cellId > 559)
                return false;

            _account.Scripts.ActionsManager.EnqueueAction(new FromBotMoveToCellAction(groupMng,idMng,cellId), true);
            return true;
        }
        public bool GroupMapTeleportation(int destId)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new GroupMapTeleportationAction(destId), true);
            return true;
        }

        public bool UseById(int elementId, int skillInstanceUid = -1)
        {
            var interactive = _account.Game.Map.GetInteractiveElement(elementId);

            if (interactive?.Usable == false)
                return false;

            _account.Scripts.ActionsManager.EnqueueAction(new UseByIdAction(elementId, skillInstanceUid));
            return true;
        }

        public bool Use(short elementCellId, int skillInstanceUid = -1)
        {
            if (elementCellId < 0 || elementCellId > 559)
                return false;

            var interactive = _account.Game.Managers.Interactives.GetElementOnCell(elementCellId);

            if (interactive?.Usable == false)
                return false;

            _account.Scripts.ActionsManager.EnqueueAction(new UseAction(elementCellId, skillInstanceUid), true);
            return true;
        }

        public bool UseLockedHouse(short doorCellid, string lockCode)
        {
            if (doorCellid < 0 || doorCellid > 559)
                return false;

            var interactive = _account.Game.Managers.Interactives.GetElementOnCell(doorCellid);
            if (interactive?.Usable == false)
                return false;

            _account.Scripts.ActionsManager.EnqueueAction(new UseLockedHouseAction(doorCellid, lockCode));
            _account.Scripts.ActionsManager.EnqueueAction(new WaitMapChangeAction(20000), true);
            return true;
        }

        public bool UseLockedStorage(short elementCellId, string lockCode)
        {
            if (elementCellId < 0 || elementCellId > 559)
                return false;

            var lockedStorage = _account.Game.Map.LockedStorages.FirstOrDefault(ls => ls.CellId == elementCellId);
            if (lockedStorage?.Element.Usable == false)
                return false;

            _account.Scripts.ActionsManager.EnqueueAction(new UseLockedStorageAction(elementCellId, lockCode), true);
            return true;
        }

        public bool SaveZaap()
        {
            if (_account.Game.Map.Zaap == null)
            {
                _account.Logger.LogWarning("TeleportablesManager", LanguageManager.Translate("537"));
                return false;
            }

            _account.Scripts.ActionsManager.EnqueueAction(new SaveZaapAction(), true);
            return true;
        }

        public bool UseZaap(uint destinationMapId)
        {
            if (_account.Game.Map.Zaap == null)
            {
                _account.Logger.LogWarning("TeleportablesManager", LanguageManager.Translate("537"));
                return false;
            }

            _account.Scripts.ActionsManager.EnqueueAction(
                new UseTeleportableAction(Teleportables.ZAAP, destinationMapId), true);
            return true;
        }

        public bool UseZaapi(uint destinationMapId)
        {
            if (_account.Game.Map.Zaapi == null)
            {
                _account.Logger.LogWarning("TeleportablesManager", LanguageManager.Translate("539"));
                return false;
            }

            _account.Scripts.ActionsManager.EnqueueAction(
                new UseTeleportableAction(Teleportables.ZAAPI, destinationMapId), true);
            return true;
        }

        public void WaitMapChange(uint delay = 5000)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new WaitMapChangeAction(delay), true);
        }

        public void JoinFriend(string name)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new JoinFriendAction(name), true);
        }

        public bool OnCell(short cellId)
        {
            return _account.Game.Map.PlayedCharacter.CellId == cellId;
        }

        public bool OnMap(string coords)
        {
            return _account.Game.Map.IsOnMap(coords);
        }

        public string CurrentPos()
        {
            return _account.Game.Map.CurrentPosition;
        }

        public string CurrentMapId()
        {
            return _account.Game.Map.Id.ToString();
        }

        public string Area()
        {
            return _account.Game.Map.Area;
        }

        public string SubArea()
        {
            return _account.Game.Map.SubArea;
        }
        #endregion

        #region Group API

        public bool MoveToCell(string username, short cellId)
        {
            if (cellId < 0 || cellId > 559)
                return false;

            var source = BubbleBotMain.Instance.GetAccountByUsername(username);

            _account.Scripts.ActionsManager.EnqueueAction(new MoveToCellBotAction(source, cellId), true);

            return true;
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

        ~MapAPI()
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