using BubbleBot.Protocol.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using BubbleBot.Core.Enums;
using ExtensionsEnum = BubbleBot.Protocol.Server.Enums.Extensions;
using BubbleBot.Data;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Messages;
using BubbleBot.Server.Messages;
using BubbleBot.Protocol.Enums;
using BubbleBot.Core.Accounts.InGame.Character.Inventory;

namespace BubbleBot.Core.Accounts.Extensions.UpdateServerHDV
{
    public class UpdateServerHDVExtension : IDisposable
    {

        private Account _account;
        public bool _running;

        private const int _timeout = 3600000;             //Temps en heures avant de recommencer la recupération 

        //Load item to update 
        private Dictionary<uint, string> ItemsToUpdate;
        private const string FilePath = @"Parameters\Bid\itemToUpdate.txt";
        private const string TxtSeparator = ";";

        public bool Enabled { get; set; }

        public UpdateServerHDVExtension(Account account)
        {
            _account = account;
            Enabled = false;
        }

        public void Initialize()
        {
           // if (_running)
           //     return;

            Console.WriteLine("Démarrage de la collecte des données de l'HDV.");
            
            _running = true;

            //Recuperation des items a update 
            ItemsToUpdate = new Dictionary<uint, string>();
            TakeItemToUpdate();

            while (Enabled == true)
                StartCollect();

            //_timer = new Timer(Timer_Callback, null, Timeout.Infinite, Timeout.Infinite);
        }
        private void TakeItemToUpdate()
        {
            if (TxtSeparator.Length == 0 || FilePath.Length == 0 || !File.Exists(FilePath))
            {
                _account.Logger.LogDebug("UPDATE-SERVER", "Erreur dans la configuration !");
                return;
            }

            var lines = File.ReadAllLines(FilePath);

            

            for (var i = 0; i < lines.Length; i++)
            {
                var infos = lines[i].Split(new[] { TxtSeparator }, StringSplitOptions.RemoveEmptyEntries);

                if (infos.Length < 2)
                    continue;
                Console.WriteLine(infos[0] + "   " + infos[1]);
                uint.TryParse(infos[0], out var tempitemid);
                ItemsToUpdate.Add(tempitemid, infos[1]);
            }

        }

        private void StartCollect()
        {
            if (!_running)
                return;

            if (_account.State != Enums.AccountStates.NONE)
            {
                _account.Logger.LogError("UPDATE-SERVER", "Le compte est actuellement sur une autre occupation, fin de la collecte.");
                return;
            }

               if (StartBuying().Result == false)
               {
                   _account.Logger.LogError("UPDATE-SERVER", "Erreur lors de l'ouverture de l'HDV.");
                   return;
               }
               Console.WriteLine("HDV OPEN");
             

            //Pour chaque items a update 
            for (int i = 0; i < ItemsToUpdate.Count; i++)
            {
                try
                {
                   // Console.WriteLine(i.ToString());
                    List<BidExchangerObjectInfo> itemsSelectedInHDV = new List<BidExchangerObjectInfo>();
                   // Console.WriteLine(ItemsToUpdate.ElementAt(i).Key.ToString());

                    itemsSelectedInHDV = _account.Game.Bid.GetListOfItem(ItemsToUpdate.ElementAt(i).Key);
                   // Console.WriteLine("Nombre item :" + itemsSelectedInHDV.Count.ToString());

                    if (itemsSelectedInHDV == null || itemsSelectedInHDV.Count <= 0)
                    {
                        //Console.WriteLine("Nothing...");
                        continue;
                    }

                    int itemAveragePrice = -1;
                    int itemPriceLot1 = -1;
                    int itemPriceLot10 = -1;
                    int itemPriceLot100 = -1;

                    //Recup prix moyen
                    itemAveragePrice = _account.Game.Bid.GetAverageItemPrice(ItemsToUpdate.ElementAt(i).Key);

                    BidExchangerObjectInfo tempItem = new BidExchangerObjectInfo();
                    //lot 1
                    tempItem = itemsSelectedInHDV.OrderBy(o => o.Prices[0]).First();
                    if (tempItem != null)
                        itemPriceLot1 = (int)tempItem.Prices[0];

                    //lot 10
                    tempItem = itemsSelectedInHDV.OrderBy(o => o.Prices[1]).First();
                    if (tempItem != null)
                        itemPriceLot10 = (int)tempItem.Prices[1];


                    //lot 100
                    tempItem = itemsSelectedInHDV.OrderBy(o => o.Prices[2]).First();
                    if (tempItem != null)
                        itemPriceLot100 = (int)tempItem.Prices[2];
                    //BubbleBotMain.Instance.Server.SendMessage(new CollectHDVMessage((int)ItemsToUpdate.ElementAt(i).Key, ItemsToUpdate.ElementAt(i).Value, _account.Game.Server.Name, 100, (int)tempItem.Prices[2], (int)itemAveragePrice));

                    BubbleBotMain.Instance.Server.SendMessage(new CollectHDVMessage((int)ItemsToUpdate.ElementAt(i).Key, ItemsToUpdate.ElementAt(i).Value, _account.Game.Server.Name, itemPriceLot1, itemPriceLot10, itemPriceLot100, (int)itemAveragePrice));
                   // ItemsKeeped.Add(ItemsToUpdate.ElementAt(i).Key, ItemsToUpdate.ElementAt(i).Value);
                    _account.Logger.LogInfo("UPDATE-SERVER", "ITEM: " + ItemsToUpdate.ElementAt(i).Value + "PRIX-MOYEN: " + itemAveragePrice.ToString() + "  PRIX-LOT-1: " + itemPriceLot1.ToString() + "  PRIX-LOT-10: " + itemPriceLot10.ToString() + "  PRIX-LOT-100: " + itemPriceLot100.ToString());
                    if(!Enabled)
                    {
                        if (_account.IsInDialog())
                        {
                            _account.LeaveDialog();
                        }
                        _account.Logger.LogInfo("UPDATE-SERVER", "Fin de la collecte.");
                        _running = false;
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Problème dans la recherche de l'item....");
                }
            }

            if(_account.IsInDialog())
            {
                _account.LeaveDialog();
            }

            _account.Logger.LogInfo("UPDATE-SERVER", "Fin de la collecte.");
            /*
            Console.WriteLine("NOMBRE ELEMENTS RECUPERER : " + ItemsKeeped.Count.ToString());

            foreach (var item in ItemsKeeped)
            {
                Console.WriteLine(item.Key.ToString() + ";" + item.Value);
            }
            */
        }

        private async Task<bool> StartBuying()
        {
            if (_account.IsBusy || !BubbleBotMain.Instance.Server.IsSubscribedToTouch ||
                !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                return false;
            
            _account.Network.SendMessage(new NpcGenericActionRequestMessage(0, 6, _account.Game.Map.Id));

            await Task.Delay(2000);
            return true;
        }

        /*
        private async void RestartCollect(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("{0:HH:mm:ss.fff} Démarrage de la collecte des données de l'HDV.",e.SignalTime);

            if (_running == true)
            {




            }

        }
        */


        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                   // Configuration.Dispose();
                   // _seekChannelTimer.Dispose();
                    //_salesChannelTimer.Dispose();
                    //_generalChannelTimer.Dispose();
                }

               // Configuration = null;
               // _seekChannelTimer = null;
               // _salesChannelTimer = null;
               //_generalChannelTimer = null;
                _account = null;
                ItemsToUpdate = null;

                disposedValue = true;
            }
        }

        ~UpdateServerHDVExtension()
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
