using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Messages;
using BubbleBot.Protocol.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Enums;
using ExtensionsEnum = BubbleBot.Protocol.Server.Enums.Extensions;

namespace BubbleBot.Core.Accounts.InGame.Bid
{
    public class BidGame : IClearable, IDisposable
    {

        // Fields
        private Account _account;
        private TaskCompletionSource<List<BidExchangerObjectInfo>> _itemDescriptionTcs;
        private uint _lastSearchedGID;


        // Properties
        public uint MaxItemsPerAccount { get; private set; }
        public List<ObjectItemToSellInBid> ObjectsInSale { get; private set; }
        public List<BidUserCondition> UserCondition { get; private set; }

        // Events
        public event Action StartedBuying;
        public event Action StartedSelling;
        public event Action BidLeft;


        // Constructor
        public BidGame(Account account)
        {
            _account = account;
            UserCondition = new List<BidUserCondition>();
        }


        public bool StartBuying()
        {
            if (_account.IsBusy || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return false;

            _account.Network.SendMessage(new NpcGenericActionRequestMessage(0, 6, _account.Game.Map.Id));
            return true;
        }

        public uint GetItemPrice(uint gid, uint lot)
        {
            if (_account.State != AccountStates.BUYING || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return 0;

            var cheapestItem = GetCheapestItem(gid, lot);

            // In case the item wasn't found
            if (cheapestItem == null)
                return 0;

            uint minPriceTake = cheapestItem.Prices[lot == 1 ? 0 : lot == 10 ? 1 : 2];

            if(minPriceTake >= 1 )
            {
                return minPriceTake;
            }
            else
            {
                return 0;
            }
        }

        public uint[] GetItemPrices(uint gid)
        {
            if (_account.State != AccountStates.BUYING || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return null;

            if (!InitializeGetItemPrice(gid))
                return null;

            // Item not found in bid
            if (_itemDescriptionTcs.Task.Result == null || _itemDescriptionTcs.Task.Result.Count == 0)
                return new uint[] { 0, 0, 0 };

            return new[]
            {
                _itemDescriptionTcs.Task.Result.Min(o => o.Prices[0]),
                _itemDescriptionTcs.Task.Result.Min(o => o.Prices[1]),
                _itemDescriptionTcs.Task.Result.Min(o => o.Prices[2])
            };
        }

        public bool BuyItem(uint gid, uint lot)
        {
            if (_account.State != AccountStates.BUYING || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return false;

            var cheapestItem = GetCheapestItem(gid, lot);

            // In case the item wasn't found
            if (cheapestItem == null)
                return false;

            uint price = cheapestItem.Prices[lot == 1 ? 0 : lot == 10 ? 1 : 2];

            // Not enough kamas
            if (price > _account.Game.Character.Inventory.Kamas)
            {
                _account.Logger.LogWarning(LanguageManager.Translate("102"), LanguageManager.Translate("103", gid, lot, price));
                return false;
            }

            _account.Network.SendMessage(new ExchangeBidHouseBuyMessage(cheapestItem.ObjectUID, lot, price));
            return true;
        }

        public bool StartSelling()
        {
            if (_account.IsBusy || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return false;

            _account.Network.SendMessage(new NpcGenericActionRequestMessage(0, 5, _account.Game.Map.Id));
            return true;
        }

        public bool SellItem(uint gid, uint lot, uint price, int unsoldDelay = 0)
        {
            if (_account.State != AccountStates.SELLING || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return false;

            var item = _account.Game.Character.Inventory.GetObjectByGID((int)gid);

            if (item == null || item.Quantity < lot)
            {
                _account.Logger.LogWarning(LanguageManager.Translate("102"), LanguageManager.Translate("104", gid, item?.Quantity ?? 0, lot));
                return false;
            }

            if(unsoldDelay == 0)
                _account.Logger.LogInfo(LanguageManager.Translate("102"), LanguageManager.Translate("105", lot, item.Name, price));
            else if(unsoldDelay > 0)
                _account.Logger.LogInfo(LanguageManager.Translate("102"), (LanguageManager.Translate("105", lot, item.Name, price) + LanguageManager.Translate("657", unsoldDelay)));
            else
                _account.Logger.LogInfo(LanguageManager.Translate("102"), LanguageManager.Translate("658"));

            _account.Network.SendMessage(new ExchangeObjectMovePricedMessage(item.UID, (int)lot, (int)price));
            // si on reçoit le message bien mit en vente...
            return true;
        }

        public bool RemoveItemInSale(uint uid)
        {
            if (_account.State != AccountStates.SELLING || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return false;

            var itemInSale = ObjectsInSale.FirstOrDefault(o => o.ObjectUID == uid);

            if (itemInSale == null)
                return false;

            _account.Network.SendMessage(new ExchangeObjectMoveMessage(itemInSale.ObjectUID, (int)itemInSale.Quantity * -1)
            {
                Price = itemInSale.ObjectPrice
            });

            _account.Logger.LogInfo(LanguageManager.Translate("102"), LanguageManager.Translate("106", itemInSale.Quantity, itemInSale.ObjectGID));
            return true;
        }

        public bool EditItemInSalePrice(uint uid, uint newPrice)
        {
            if (_account.State != AccountStates.SELLING || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return false;

            var itemInSale = ObjectsInSale.FirstOrDefault(o => o.ObjectUID == uid);

            if (itemInSale == null)
                return false;

            if (!RemoveItemInSale(uid))
                return false;

            // Then we re-sell it with the new price
            var t = new Task(async () =>
            {
                await Task.Delay(1500);
                SellItem(itemInSale.ObjectGID, itemInSale.Quantity, newPrice);
            });
            t.RunSynchronously();

            return true;
        }

        public void Clear()
        {
            _itemDescriptionTcs = null;
            _lastSearchedGID = 0;
        }

        #region Updates

        public void Update(ExchangeStartedBidBuyerMessage message)
        {
            if (!BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return;

            _account.State = AccountStates.BUYING;
            MaxItemsPerAccount = message.BuyerDescriptor.MaxItemPerAccount;

            StartedBuying?.Invoke();
        }

        public void Update(ExchangeTypesItemsExchangerDescriptionForUserMessage message)
        {
            if (!BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return;
        
            _itemDescriptionTcs?.SetResult(message.ItemTypeDescriptions);
        }

        public void Update(ExchangeStartedBidSellerMessage message)
        {
            if (!BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return;

            _account.State = AccountStates.SELLING;
            MaxItemsPerAccount = message.SellerDescriptor.MaxItemPerAccount;
            ObjectsInSale = message.ObjectsInfos;

            StartedSelling?.Invoke();
        }

        public void Update(ExchangeErrorMessage message)
        {
            if (!BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return;

            if (message.ErrorType != 11 || _itemDescriptionTcs == null)
                return;

            _itemDescriptionTcs.SetResult(null);
        }

        public void Update(ExchangeLeaveMessage message)
        {
            if (!BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return;

            if (_account.State != AccountStates.BUYING && _account.State != AccountStates.SELLING)
                return;

            _account.State = AccountStates.NONE;
            _itemDescriptionTcs = null;
            _lastSearchedGID = 0;
            MaxItemsPerAccount = 0;

            BidLeft?.Invoke();
        }

        #endregion

        private BidExchangerObjectInfo GetCheapestItem(uint gid, uint lot)
        {
            if (_account.State != AccountStates.BUYING || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return null;

            if (!InitializeGetItemPrice(gid))
                return null;

            // Item not found in bid
            if (_itemDescriptionTcs.Task.Result == null || _itemDescriptionTcs.Task.Result.Count == 0)
                return null;

            int index = lot == 1 ? 0 : lot == 10 ? 1 : 2;

            return _itemDescriptionTcs.Task.Result.OrderBy(o => o.Prices[index]).First();
        }

        private bool InitializeGetItemPrice(uint gid)
        {
            if (!BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return false;

            var item = DataManager.Get<Items>((int)gid);

            if (item == null)
                return false;

            if (_lastSearchedGID != gid || _itemDescriptionTcs == null)
            {
                _itemDescriptionTcs = new TaskCompletionSource<List<BidExchangerObjectInfo>>();
                _account.Network.SendMessage(new ExchangeBidHouseSearchMessage((uint)item.TypeId, gid));
                
                _itemDescriptionTcs.Task.Wait();
                _lastSearchedGID = gid;
            }

            return true;
        }

        public bool ExtendedBuyItem(uint gid, uint lot,uint maxPrice)
        {
            if (_account.State != AccountStates.BUYING || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return false;

            var cheapestItem = GetAllItemInSell(gid, lot);

            // In case the item wasn't found
            if (cheapestItem.Count <= 0)
                return false;

            List<BidExchangerObjectInfo> filteredItem = new List<BidExchangerObjectInfo>();

            foreach (var item in cheapestItem)  //Pour chaque item du type selectionner
            {
                bool isVerified = false;
                int conditionChecked = 0;
                int nbConditionToCheck = UserCondition.Count;

                foreach (var condition in UserCondition)
                {
                    condition.Checked = false;
                }

                for (int effId = 0; effId < item.Effects.Count;effId++)  //Pour chaque effet de l'item
                {
                    if (nbConditionToCheck > 0)
                    {
                        foreach (var condition in UserCondition)    //Pour chaque condition selectionner par l'utilisateur
                        {
                            ObjectEffectInteger oei = new ObjectEffectInteger();
                            oei.Value = item.Effects[effId].Value;
                            oei.ActionId = item.Effects[effId].ActionId;
                            if (condition.ItemEffectsId == oei.ActionId) //Si la condition s'applique a l'effet selectionner
                            {
                                if (condition.BidConditionChecker((int)oei.Value)) //Verifie si la condition est valide 
                                {
                                    isVerified = true;
                                    conditionChecked++;
                                }
                                else
                                {
                                    isVerified = false;
                                    effId = item.Effects.Count + 5;
                                    break;
                                }
                            }
                             
                        }
                    }
                }
                if (isVerified == true)
                {
                    foreach (var condition in UserCondition)
                    {
                        if (condition.Checked == false)
                        {
                            if (condition.BidConditionChecker(0))
                            {
                                conditionChecked++;
                            }
                        }
                    }
                    if(conditionChecked == nbConditionToCheck)
                    {
                        filteredItem.Add(item);
                    }
                }
            }

            //Si on a recuperer des items 
            if (filteredItem.Count > 0)
            {
                BidExchangerObjectInfo itemToBuy = new BidExchangerObjectInfo();

                int index = lot == 1 ? 0 : lot == 10 ? 1 : 2;
                uint price = 0;
                itemToBuy = filteredItem.OrderBy(o => o.Prices[index]).First();
                price = itemToBuy.Prices[index];
                if (price > maxPrice && maxPrice != 0)
                {
                    _account.Logger.LogWarning(LanguageManager.Translate("102"), LanguageManager.Translate("647", gid, lot, price));
                    UserCondition = new List<BidUserCondition>();
                    return false;
                }
                // Not enough kamas
                if (price > _account.Game.Character.Inventory.Kamas)
                {
                    _account.Logger.LogWarning(LanguageManager.Translate("102"), LanguageManager.Translate("103", gid, lot, price));
                    UserCondition = new List<BidUserCondition>();
                    return false;
                }

                if(itemToBuy.Prices[index] > 0)
                {
                    _account.Logger.LogInfo(LanguageManager.Translate("102"), LanguageManager.Translate("648", gid, lot, price));
                    _account.Network.SendMessage(new ExchangeBidHouseBuyMessage(itemToBuy.ObjectUID, lot, price));
                    UserCondition = new List<BidUserCondition>();
                    return true;
                }
            }
            _account.Logger.LogWarning(LanguageManager.Translate("102"), LanguageManager.Translate("649"));
            UserCondition = new List<BidUserCondition>();
            return false;
        }

        private List<BidExchangerObjectInfo> GetAllItemInSell(uint gid, uint lot)
        {
            if (_account.State != AccountStates.BUYING || !BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return null;

            if (!InitializeGetItemPrice(gid))
                return null;

            // Item not found in bid
            if (_itemDescriptionTcs.Task.Result == null || _itemDescriptionTcs.Task.Result.Count == 0)
                return null;

            int index = lot == 1 ? 0 : lot == 10 ? 1 : 2;

            return _itemDescriptionTcs.Task.Result;
        }

        public bool AddBuyItemCondition(int EffectsId, string conditionType , int EffectsValue)
        {
            UserCondition.Add(new BidUserCondition(EffectsId, conditionType, EffectsValue));
            return true;
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                ObjectsInSale?.Clear();
                ObjectsInSale = null;
                _account = null;
                UserCondition = null;

                disposedValue = true;
            }
        }

        ~BidGame() => Dispose(false);

        public void Dispose() => Dispose(true);

        #endregion

    }
}
