using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Character.Inventory;
using BubbleBot.Core.Enums;
using BubbleBot.Protocol.Enums;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Accounts.InGame.Exchange
{

    public class ExchangeGame : IDisposable
    {
        // Fields
        private Account _account;
        private uint _step;
        public static List<uint> AuthorizedPlayersList { get; set; }


        // Constructor
        public ExchangeGame(Account account)
        {
            _account = account;

            Objects = new List<ObjectEntry>();
            RemoteObjects = new List<ObjectEntry>();

            if (AuthorizedPlayersList == null)
            {
                AuthorizedPlayersList = new List<uint>();
            }
        }


        // Properties
        public List<ObjectEntry> Objects { get; private set; }
        public List<ObjectEntry> RemoteObjects { get; private set; }
        public uint Kamas { get; private set; }
        public uint RemoteKamas { get; private set; }
        public uint CurrentWeight { get; private set; }
        public uint MaxWeight { get; private set; }
        public uint RemoteCurrentWeight { get; private set; }
        public uint RemoteMaxWeight { get; private set; }
        public int RemoteCharacterId { get; private set; }
        public bool IsReady { get; private set; }
        public bool RemoteIsReady { get; private set; }

        public int WeightPercent => (int)((double)CurrentWeight / MaxWeight * 100);
        public int RemoteWeightPercent => (int)((double)RemoteCurrentWeight / RemoteMaxWeight * 100);


        // Events
        public event Action<int> ExchangeRequested;
        public event Action ExchangeStarted;
        public event Action ExchangeContentChanged;
        public event Action RemoteReady;
        public event Action ExchangeLeft;

        public bool StartExchange(int id)
        {
            if (_account.IsBusy)
                return false;

            if (_account.Game.Map.Players.FirstOrDefault(p => p.Id == id) == null)
                return false;

            _account.Network.SendMessage(new ExchangePlayerRequestMessage(1, (uint)id));
            return true;
        }
        public async Task<bool> FromBotPutAllItems(string botGroupMng, string botIdMng)
        {
            foreach (var acc in BubbleBotMain.Instance.ConnectedAccounts)
            {
                if (acc.IsGroupChief && acc.HasGroup)
                {
                    foreach (var member in acc.Group.Members)
                    {
                        if (member.AccountConfig.Nickname == botGroupMng && member.AccountConfig.Identifiant == botIdMng)
                        {
                            if (member.State != AccountStates.EXCHANGE)
                                return false;

                            member.Logger.LogDebug(LanguageManager.Translate("117"), LanguageManager.Translate("531"));

                            foreach (var obj in member.Game.Character.Inventory.Equipements)
                            {
                                if (!obj.Exchangeable || obj.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                                    continue;

                                member.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int)obj.Quantity));
                                await Task.Delay(600);
                            }

                            foreach (var obj in member.Game.Character.Inventory.Consumables)
                            {
                                if (!obj.Exchangeable || obj.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                                    continue;

                                member.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int)obj.Quantity));
                                await Task.Delay(600);
                            }

                            foreach (var obj in member.Game.Character.Inventory.Resources)
                            {
                                if (!obj.Exchangeable || obj.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                                    continue;

                                member.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int)obj.Quantity));
                                await Task.Delay(600);
                            }

                            member.Logger.LogInfo(LanguageManager.Translate("117"), LanguageManager.Translate("532"));
                            return true;
                        }

                    }
                }

                if (acc.AccountConfig.Nickname == botGroupMng && acc.AccountConfig.Identifiant == botIdMng)
                {
                    if (acc.State != AccountStates.EXCHANGE)
                        return false;

                    acc.Logger.LogDebug(LanguageManager.Translate("117"), LanguageManager.Translate("531"));

                    foreach (var obj in acc.Game.Character.Inventory.Equipements)
                    {
                        if (!obj.Exchangeable || obj.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                            continue;

                        acc.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int)obj.Quantity));
                        await Task.Delay(600);
                    }

                    foreach (var obj in acc.Game.Character.Inventory.Consumables)
                    {
                        if (!obj.Exchangeable || obj.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                            continue;

                        acc.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int)obj.Quantity));
                        await Task.Delay(600);
                    }

                    foreach (var obj in acc.Game.Character.Inventory.Resources)
                    {
                        if (!obj.Exchangeable || obj.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                            continue;

                        acc.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int)obj.Quantity));
                        await Task.Delay(600);
                    }

                    acc.Logger.LogInfo(LanguageManager.Translate("117"), LanguageManager.Translate("532"));
                    return true;
                }

            }
            return false;
        }
        public bool FromBotPutItem(string botGroupMng, string botIdMng, int gid, uint quantity)
        {
            foreach (var acc in BubbleBotMain.Instance.ConnectedAccounts)
            {
                if (acc.IsGroupChief && acc.HasGroup)
                {
                    foreach (var member in acc.Group.Members)
                    {
                        if (member.AccountConfig.Nickname == botGroupMng && member.AccountConfig.Identifiant == botIdMng)
                        {
                            if (member.State != AccountStates.EXCHANGE)
                                return false;

                            var obj = member.Game.Character.Inventory.GetObjectByGID(gid);

                            if (obj == null)
                                return false;

                            quantity = quantity == 0 ? obj.Quantity :
                                quantity > obj.Quantity ? obj.Quantity : quantity;

                            member.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int)quantity));
                            member.Logger.LogInfo(LanguageManager.Translate("117"),
                                LanguageManager.Translate("118", quantity, obj.Name));
                            return true;
                        }

                    }
                }

                if (acc.AccountConfig.Nickname == botGroupMng && acc.AccountConfig.Identifiant == botIdMng)
                {
                    if (acc.State != AccountStates.EXCHANGE)
                        return false;

                    var obj = acc.Game.Character.Inventory.GetObjectByGID(gid);

                    if (obj == null)
                        return false;

                    quantity = quantity == 0 ? obj.Quantity :
                        quantity > obj.Quantity ? obj.Quantity : quantity;

                    acc.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int)quantity));
                    acc.Logger.LogInfo(LanguageManager.Translate("117"),LanguageManager.Translate("118", quantity, obj.Name));
                    return true;
                }

            }
            return false;
        }

        public async Task<bool> FromBotExchangeByName(string botGroupMng, string botIdMng, string targetName)
        {
            await Task.Delay(1);

            if (_account.IsBusy)
                return false;

            if (botGroupMng == "" && botIdMng == "")
                return false;

            //Si le compte est en groupe on cherche le compte où executer l'action 
                foreach (var acc in BubbleBotMain.Instance.ConnectedAccounts)
                {
                    if(acc.IsGroupChief && acc.HasGroup)
                    {
                        foreach (var member in acc.Group.Members)
                        {
                            if (member.AccountConfig.Nickname == botGroupMng && member.AccountConfig.Identifiant == botIdMng)
                            {
                                if (member.Game.Map.Players.FirstOrDefault(p => p.Name == targetName) == null)
                                    return false;

                                member.Network.SendMessage(new ExchangePlayerRequestMessage(1, (uint)member.Game.Map.Players.FirstOrDefault(p => p.Name == targetName).Id));
                                SpinWait.SpinUntil(() => member.State == AccountStates.EXCHANGE, TimeSpan.FromSeconds(30));
                                return true;
                            }

                        }
                    }

                    if (acc.AccountConfig.Nickname == botGroupMng && acc.AccountConfig.Identifiant == botIdMng)
                    {
                        if (acc.Game.Map.Players.FirstOrDefault(p => p.Name == targetName) == null)
                            return false;

                        acc.Network.SendMessage(new ExchangePlayerRequestMessage(1, (uint)acc.Game.Map.Players.FirstOrDefault(p => p.Name == targetName).Id));
                        SpinWait.SpinUntil(() => acc.State == AccountStates.EXCHANGE, TimeSpan.FromSeconds(30));
                        return true;
                    }
                }

            return false;
        }

        public bool StartExchangeByName(string targetName)
        {
            if (_account.IsBusy)
                return false;

            if (_account.Game.Map.Players.FirstOrDefault(p => p.Name == targetName) == null)
                return false;

            _account.Network.SendMessage(new ExchangePlayerRequestMessage(1,
                (uint) _account.Game.Map.Players.FirstOrDefault(p => p.Name == targetName).Id));

            SpinWait.SpinUntil(() => _account.State == AccountStates.EXCHANGE, TimeSpan.FromSeconds(30));

            return true;
        }

        public async Task<bool> StartExchangeGroupByName(string targetName)
        {
            if (!_account.IsGroupChief)
                return false;

            for (int i = 0; i < _account.Group.Members.Count; i++)
            {
                _account.Group.Members[i].Game.Exchange.StartExchangeByName(targetName);

                SpinWait.SpinUntil(() => _account.Group.Members[i].State == AccountStates.EXCHANGE, TimeSpan.FromSeconds(30));
                SpinWait.SpinUntil(() => _account.Group.Members[i].State != AccountStates.EXCHANGE, TimeSpan.FromSeconds(30));

                await Task.Delay(500);
            }

            _account.Game.Exchange.StartExchangeByName(targetName);

            SpinWait.SpinUntil(() => _account.State == AccountStates.EXCHANGE, TimeSpan.FromSeconds(30));
            SpinWait.SpinUntil(() => _account.State != AccountStates.EXCHANGE, TimeSpan.FromSeconds(30));

            await Task.Delay(500);

            return true;
        }

        public bool SendReady()
        {
            if (_account.State != AccountStates.EXCHANGE)
                return false;

            _account.Network.SendMessage(new ExchangeReadyMessage(true, _step));
            _account.Logger.LogInfo(LanguageManager.Translate("117"), LanguageManager.Translate("116"));
            return true;
        }

        public bool PutItem(int gid, uint quantity)
        {
            if (_account.State != AccountStates.EXCHANGE)
                return false;

            var obj = _account.Game.Character.Inventory.GetObjectByGID(gid);

            if (obj == null)
                return false;

            quantity = quantity == 0 ? obj.Quantity :
                quantity > obj.Quantity ? obj.Quantity : quantity;

            _account.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int) quantity));
            _account.Logger.LogInfo(LanguageManager.Translate("117"),
                LanguageManager.Translate("118", quantity, obj.Name));
            return true;
        }

        public bool AddPlayerAuthorization(uint playerId)
        {
            AuthorizedPlayersList.Add(playerId);
            return true;
        }

        public bool RemoveItem(int gid, uint quantity)
        {
            if (_account.State != AccountStates.EXCHANGE)
                return false;

            var obj = Objects.FirstOrDefault(o => o.GID == gid);

            if (obj == null)
                return false;

            quantity = quantity == 0 ? obj.Quantity :
                quantity > obj.Quantity ? obj.Quantity : quantity;

            _account.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int) quantity * -1));
            _account.Logger.LogInfo(LanguageManager.Translate("117"),
                LanguageManager.Translate("119", quantity, obj.Name));
            return true;
        }

        public async Task<bool> PutAllItems()
        {
            if (_account.State != AccountStates.EXCHANGE)
                return false;

            _account.Logger.LogDebug(LanguageManager.Translate("117"), LanguageManager.Translate("531"));

            foreach (var obj in _account.Game.Character.Inventory.Equipements)
            {
                if (!obj.Exchangeable || obj.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                    continue;

                _account.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int) obj.Quantity));
                await Task.Delay(600);
            }

            foreach (var obj in _account.Game.Character.Inventory.Consumables)
            {
                if (!obj.Exchangeable || obj.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                    continue;

                _account.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int) obj.Quantity));
                await Task.Delay(600);
            }

            foreach (var obj in _account.Game.Character.Inventory.Resources)
            {
                if (!obj.Exchangeable || obj.Position != CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED)
                    continue;

                _account.Network.SendMessage(new ExchangeObjectMoveMessage(obj.UID, (int) obj.Quantity));
                await Task.Delay(600);
            }

            _account.Logger.LogInfo(LanguageManager.Translate("117"), LanguageManager.Translate("532"));
            return true;
        }
        public bool FromBotPutKamas(string botGroupMng, string botIdMng, uint quantity)
        {
            foreach (var acc in BubbleBotMain.Instance.ConnectedAccounts)
            {
                if (acc.IsGroupChief && acc.HasGroup)
                {
                    foreach (var member in acc.Group.Members)
                    {
                        if (member.AccountConfig.Nickname == botGroupMng && member.AccountConfig.Identifiant == botIdMng)
                        {
                            if (member.State != AccountStates.EXCHANGE)
                                return false;

                            quantity = quantity == 0 ? (uint)member.Game.Character.Inventory.Kamas :
                            quantity > member.Game.Character.Inventory.Kamas ? (uint)member.Game.Character.Inventory.Kamas :
                            quantity;

                            if (quantity > 0)
                            {
                                member.Logger.LogInfo(LanguageManager.Translate("117"), LanguageManager.Translate("120", quantity));
                                member.Network.SendMessage(new ExchangeObjectMoveKamaMessage((int)quantity));
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }

                    }
                }

                if (acc.AccountConfig.Nickname == botGroupMng && acc.AccountConfig.Identifiant == botIdMng)
                {
                    if (acc.State != AccountStates.EXCHANGE)
                        return false;

                    quantity = quantity == 0 ? (uint)acc.Game.Character.Inventory.Kamas :
                    quantity > acc.Game.Character.Inventory.Kamas ? (uint)acc.Game.Character.Inventory.Kamas :
                    quantity;

                    if (quantity > 0)
                    {
                        acc.Logger.LogInfo(LanguageManager.Translate("117"), LanguageManager.Translate("120", quantity));
                        acc.Network.SendMessage(new ExchangeObjectMoveKamaMessage((int)quantity));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public bool PutKamas(uint quantity)
        {
            if (_account.State != AccountStates.EXCHANGE)
                return false;

            quantity = quantity == 0 ? (uint) _account.Game.Character.Inventory.Kamas :
                quantity > _account.Game.Character.Inventory.Kamas ? (uint) _account.Game.Character.Inventory.Kamas :
                quantity;

            if (quantity > 0)
            {
                _account.Logger.LogInfo(LanguageManager.Translate("117"), LanguageManager.Translate("120", quantity));
                _account.Network.SendMessage(new ExchangeObjectMoveKamaMessage((int) quantity));
                return true;
            }

            return false;
        }

        public bool RemoveKamas(uint quantity)
        {
            if (_account.State != AccountStates.EXCHANGE)
                return false;

            quantity = quantity == 0 ? Kamas :
                quantity > Kamas ? Kamas : quantity;

            if (quantity > 0)
            {
                _account.Logger.LogInfo(LanguageManager.Translate("117"), LanguageManager.Translate("121", quantity));
                _account.Network.SendMessage(new ExchangeObjectMoveKamaMessage((int) (Kamas - quantity)));
                return true;
            }

            return false;
        }

        #region Updates

        public void Update(ExchangeRequestedTradeMessage message)
        {
            if (message.ExchangeType == 1 && message.Target == _account.Game.Character.Id)
                ExchangeRequested?.Invoke((int) message.Source);
        }

        public void Update(ExchangeStartedWithPodsMessage message)
        {
            _step = 0;
            IsReady = false;
            RemoteIsReady = false;
            _account.State = AccountStates.EXCHANGE;

            if (message.FirstCharacterId == _account.Game.Character.Id)
            {
                CurrentWeight = message.FirstCharacterCurrentWeight;
                MaxWeight = message.FirstCharacterMaxWeight;
                RemoteCurrentWeight = message.SecondCharacterCurrentWeight;
                RemoteMaxWeight = message.SecondCharacterMaxWeight;
                RemoteCharacterId = message.SecondCharacterId;
            }
            else
            {
                CurrentWeight = message.SecondCharacterCurrentWeight;
                MaxWeight = message.SecondCharacterMaxWeight;
                RemoteCurrentWeight = message.FirstCharacterCurrentWeight;
                RemoteMaxWeight = message.FirstCharacterMaxWeight;
                RemoteCharacterId = message.FirstCharacterId;
            }

            ExchangeStarted?.Invoke();
        }

        public void Update(ExchangeObjectAddedMessage message)
        {
            var newObj = new ObjectEntry(message.Object);

            if (message.Remote)
            {
                RemoteObjects.Add(newObj);
                RemoteCurrentWeight += (uint) newObj.RealWeight * newObj.Quantity;
            }
            else
            {
                Objects.Add(newObj);
                CurrentWeight += (uint) newObj.RealWeight * newObj.Quantity;
            }

            _step++;
            ExchangeContentChanged?.Invoke();
        }

        public void Update(ExchangeObjectModifiedMessage message)
        {
            var modifiedObj = message.Remote
                ? RemoteObjects.FirstOrDefault(o => o.UID == message.Object.ObjectUID)
                : Objects.FirstOrDefault(o => o.UID == message.Object.ObjectUID);

            var qtyDiff = (int) message.Object.Quantity - (int) modifiedObj.Quantity;
            modifiedObj.Update(message.Object);

            if (message.Remote)
                RemoteCurrentWeight += (uint) (qtyDiff * modifiedObj.RealWeight);
            else
                CurrentWeight += (uint) (qtyDiff * modifiedObj.RealWeight);

            _step++;
            ExchangeContentChanged?.Invoke();
        }

        public void Update(ExchangeObjectRemovedMessage message)
        {
            var removedObj = message.Remote
                ? RemoteObjects.FirstOrDefault(o => o.UID == message.ObjectUID)
                : Objects.FirstOrDefault(o => o.UID == message.ObjectUID);

            if (message.Remote)
            {
                RemoteCurrentWeight += (uint) (removedObj.Quantity * removedObj.RealWeight);
                RemoteObjects.Remove(removedObj);
            }
            else
            {
                CurrentWeight += (uint) (removedObj.Quantity * removedObj.RealWeight);
                Objects.Remove(removedObj);
            }

            _step++;
            ExchangeContentChanged?.Invoke();
        }

        public void Update(ExchangeKamaModifiedMessage message)
        {
            if (message.Remote)
                RemoteKamas = message.Quantity;
            else
                Kamas = message.Quantity;

            _step++;
            ExchangeContentChanged?.Invoke();
        }

        public void Update(ExchangeIsReadyMessage message)
        {
            if (message.Id == _account.Game.Character.Id)
            {
                IsReady = true;
            }
            else
            {
                RemoteIsReady = true;
                RemoteReady?.Invoke();
            }
        }

        public void Update(ExchangeLeaveMessage message)
        {
            if (_account.State != AccountStates.EXCHANGE)
                return;

            Objects.Clear();
            RemoteObjects.Clear();
            Kamas = RemoteKamas = 0;
            RemoteCharacterId = 0;
            CurrentWeight = MaxWeight = RemoteCurrentWeight = RemoteMaxWeight = 0;
            _step = 0;
            _account.State = AccountStates.NONE;

            ExchangeLeft?.Invoke();
        }

        #endregion

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                Objects.Clear();
                RemoteObjects.Clear();
                Objects = null;
                RemoteObjects = null;
                _account = null;

                _disposedValue = true;
            }
        }

        ~ExchangeGame()
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