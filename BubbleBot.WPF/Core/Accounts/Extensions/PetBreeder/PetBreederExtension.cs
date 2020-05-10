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
using Timer = System.Threading.Timer;
using BubbleBot.Configurations.Language;
using GalaSoft.MvvmLight;

namespace BubbleBot.Core.Accounts.Extensions.PetBreeder
{
    public class PetBreederExtension : ViewModelBase, IDisposable
    {
        private Account _account;
        private IEnumerable<ObjectEntry> inventoryItems;
        private Timer _timer;

        public bool Enabled { get; set; }
        public bool Running { get; set; }

        public PetBreederConfiguration PetBreederConfig { get; set; }

        public PetBreederExtension(Account account)
        {
            _account = account;
            Enabled = false;
            Running = false;
            PetBreederConfig = new PetBreederConfiguration(account);
            _timer = new Timer(Timer_Callback, null, Timeout.Infinite, Timeout.Infinite);
            PetBreederConfig.Load();
        }

        public async Task StartBreeding()
        {
            Enabled = true;
            Running = true;
            CheckSession();
            _timer.Change(PetBreederConfig.Interval * 60000, PetBreederConfig.Interval * 60000);
            
        }

        private async Task ProcessFeeding(List<PetsToBreedEntry> pettobreed)
        {

            if(_account.State == AccountStates.DISCONNECTED)
            {
                await _account.Connect();
                await Task.Delay(2000);
            }

            if (_account.State != AccountStates.NONE)
                return;

            Console.WriteLine("Familier feeder :");
            // for each item in configuration
            foreach (var pet in pettobreed)
            {
                inventoryItems = _account.Game.Character.Inventory.GetObjectsByGID((int)pet.PetID); //chacha : 1728

                if(inventoryItems == null)
                {
                    _account.Logger.LogError(LanguageManager.Translate("720"), LanguageManager.Translate("721",pet.NamePet));
                    continue;
                }

                ObjectEntry food = _account.Game.Character.Inventory.GetObjectByGID((int)pet.FoodID);
                if (food == null)
                {
                    _account.Logger.LogError(LanguageManager.Translate("720"), LanguageManager.Translate("722", pet.NameFood));
                    continue;
                }
                if (food.Quantity < pet.FoodQuantity)
                {
                    _account.Logger.LogError(LanguageManager.Translate("720"), LanguageManager.Translate("723", pet.NameFood));
                    continue;
                }

                foreach (var item in inventoryItems)
                {
                    _account.Network.SendMessage(new ObjectFeedMessage(item.UID, food.UID, pet.FoodQuantity));
                    await Task.Delay(400);
                }

                _account.Logger.LogDebug("Breeder", "Familiers nourri : " + pet.NamePet);

                for(int i=0; i < PetBreederConfig.PetsToBreed.Count;i++)
                {
                    if (pet.PetID == PetBreederConfig.PetsToBreed[i].PetID)
                    {
                        PetBreederConfig.PetsToBreed[i].FedderCount = pet.FedderCount + 1;
                        PetBreederConfig.PetsToBreed[i].PetLastMeal = GetTimestamp();
                        PetBreederConfig.Save();
                        break;
                    }
                }  
            }
            await Task.Delay(5000);
            Console.WriteLine("Deconnexion :");
            await _account.Network.Disconnect("Breeder");
        }

        private void CheckSession()
        {
            PetBreederConfig.Load();

            List<PetsToBreedEntry> meallingtimepet = new List<PetsToBreedEntry>();

            foreach (var pet in PetBreederConfig.PetsToBreed)
            {
                if (((GetTimestamp() - pet.PetLastMeal) > (pet.PetIntervalMeal * 3600)) || pet.PetLastMeal == 0)
                {
                    Console.WriteLine("TRUE :");
                    meallingtimepet.Add(new PetsToBreedEntry(pet.NamePet,pet.PetID,pet.NameFood, pet.FoodID,pet.FoodQuantity,pet.PetLastMeal,pet.PetIntervalMeal,pet.FedderCount));
                }
                else
                {
                    Console.WriteLine("FALSE :" + GetTimestamp().ToString());
                    Console.WriteLine("FALSE :" + pet.PetLastMeal.ToString());
                }
                    
            }
            Console.WriteLine("Check session OK :" + meallingtimepet.Count().ToString() + "    " + PetBreederConfig.PetsToBreed.Count.ToString());

            if (meallingtimepet != null && meallingtimepet.Count() > 0)
                 ProcessFeeding(meallingtimepet).ConfigureAwait(true);

        }
        private int GetTimestamp()
        {
            return (int)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }


        private void Timer_Callback(object state)
        {
            if (!Enabled)
                return;

            CheckSession();
        }




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

                disposedValue = true;
            }
        }

        ~PetBreederExtension()
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
    
