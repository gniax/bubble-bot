using System;
using System.Windows;
using System.Windows.Controls;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Accounts.Extensions.Bid;
using BubbleBot.Protocol.Data;
using BubbleBot.Server.Messages;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using BubbleBot.Data;
using System.Threading.Tasks;
using BubbleBot.Core.Accounts.Extensions.PetBreeder;

namespace BubbleBot.Views.Accounts
{
    public partial class AccountBreederUc : UserControl
    {
        // Constructor
        public AccountBreederUc()
        {
            InitializeComponent();
        }

        // Properties
        private Account Account => BubbleBotMain.Instance.SelectedAccount;


        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var objpet = await DataManager.Get<Items>((int)petGID.Value.Value);
            var objfood = await DataManager.Get<Items>((int)foodGID.Value.Value);

            

            if (objpet == null)
            {
                var window = Window.GetWindow(this) as MetroWindow;
                await window.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("708"));
                return;
            }

            if (objfood == null)
            {
                var window = Window.GetWindow(this) as MetroWindow;
                await window.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("709"));
                return;
            }

            foreach(var cheskalreadyexist in Account.Extensions.PetBreeder.PetBreederConfig.PetsToBreed)
            {
                if(cheskalreadyexist.PetID == objpet.Id)
                {
                    var window = Window.GetWindow(this) as MetroWindow;
                    await window.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("724"));
                    return;
                }
            }

            Account.Extensions.PetBreeder.PetBreederConfig.PetsToBreed.Add(new PetsToBreedEntry(objpet.NameId, (uint)objpet.Id, objfood.NameId, (uint)objfood.Id, (uint)foodQuantity.Value.Value, 0, (int)petInterval.Value.Value,0));
            Account.Extensions.PetBreeder.PetBreederConfig.Save();
        }


        private void BtnDisable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Account.Extensions.Bid.Stop();
            }
            catch (Exception ex)
            {
                Account.Logger.LogError("", ex.ToString());
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lvObjects.SelectedItem == null)
                return;

            Account.Extensions.PetBreeder.PetBreederConfig.PetsToBreed.Remove(lvObjects.SelectedItem as PetsToBreedEntry);
            Account.Extensions.PetBreeder.PetBreederConfig.Save();
        }

        private void Btn_Start_Breeding(object sender, RoutedEventArgs e)
        {

            if(Account.Extensions.PetBreeder.Enabled == false)
            {
                btnStart.Content = LanguageManager.Translate("725");
                if(Account.Extensions.PetBreeder.Running == false)
                    Task.Run(() => Account.Extensions.PetBreeder.StartBreeding());
                else
                    Account.Extensions.PetBreeder.Enabled = true;
            }
            else
            {
                btnStart.Content = LanguageManager.Translate("717");
                Account.Extensions.PetBreeder.Enabled = false;
            }
            
           

        }
    }
}