using System;
using System.Linq;
using System.Reflection;
using BubbleBot.Core.Accounts.Scripts.Actions.Inventory;
using BubbleBot.Protocol.Enums;
using MoonSharp.Interpreter;

namespace BubbleBot.Core.Accounts.Scripts.Api
{
    [MoonSharpUserData]
    [Obfuscation(Exclude = false, Feature = "-rename", ApplyToMembers = true)]
    public class InventoryAPI : IDisposable
    {
        // Fields
        private Account _account;


        // Constructor
        public InventoryAPI(Account account)
        {
            _account = account;
        }


        public int Pods()
        {
            return _account.Game.Character.Inventory.Weight;
        }

        public int PodsMax()
        {
            return _account.Game.Character.Inventory.MaxWeight;
        }

        public int PodsP()
        {
            return _account.Game.Character.Inventory.WeightPercent;
        }

        public int ItemCount(int gid)
        {
            return _account.Game.Character.Inventory.GetObjectsByGID(gid).Sum(o => (int) o.Quantity);
        }
        public int FromBotItemCount(string groupMng, string idMng, int gid)
        {
            foreach (var acc in BubbleBotMain.Instance.ConnectedAccounts)
            {
                //Si le compte est en groupe on vérifie les membres
                if (acc.IsGroupChief && acc.HasGroup)
                {
                    foreach (var member in acc.Group.Members)
                    {
                        if (member.AccountConfig.Nickname == groupMng && member.AccountConfig.Identifiant == idMng)
                        {
                            return member.Game.Character.Inventory.GetObjectsByGID(gid).Sum(o => (int)o.Quantity);
                        }
                    }
                }
                //On vérifie les comptes principal
                if (acc.AccountConfig.Nickname == groupMng && acc.AccountConfig.Identifiant == idMng)
                {
                   return acc.Game.Character.Inventory.GetObjectsByGID(gid).Sum(o => (int)o.Quantity);
                }
            }
            return -1;
        }
        public int ItemWeight(int gid)
        {
            return _account.Game.Character.Inventory.GetObjectByGID(gid)?.RealWeight ?? 0;
        }

        public bool UseItem(int gid, uint quantity = 1)
        {
            var item = _account.Game.Character.Inventory.GetObjectByGID(gid);

            if (item == null)
                return false;

            _account.Scripts.ActionsManager.EnqueueAction(new UseItemAction(gid, quantity), true);
            return true;
        }

        public bool EquipItem(int gid)
        {
            var item = _account.Game.Character.Inventory.GetObjectByGID(gid);

            if (item == null || item.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                return false;

            _account.Scripts.ActionsManager.EnqueueAction(new EquipItemAction(gid), true);
            return true;
        }

        public bool UnEquipItem(int gid)
        {
            var item = _account.Game.Character.Inventory.GetObjectByGID(gid);

            if (item == null || item.Position == CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                return false;

            _account.Scripts.ActionsManager.EnqueueAction(new UnEquipItemAction(gid), true);
            return true;
        }

        public bool DropItem(int gid, uint quantity = 1)
        {
            var item = _account.Game.Character.Inventory.GetObjectByGID(gid);

            if (item == null)
                return false;

            _account.Scripts.ActionsManager.EnqueueAction(new DropItemAction(gid, quantity), true);
            return true;
        }

        public bool DeleteItem(int gid, uint quantity)
        {
            var item = _account.Game.Character.Inventory.GetObjectByGID(gid);

            if (item == null)
                return false;
            _account.Scripts.ActionsManager.EnqueueAction(new DeleteItemAction(gid, quantity), true);
            return true;
        }

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

        ~InventoryAPI()
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