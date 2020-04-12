using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BubbleBot.Views.Accounts
{
    public partial class AccountSubscriptionUc : UserControl
    {
        // Properties
        public string subscriptionText = "?";
        private BackgroundWorker bgWorker;
        private bool bgWorkerWorking = false;
        private Account Account => BubbleBotMain.Instance.SelectedAccount;

        private System.Timers.Timer showTimeout;
        // Constructor
        public AccountSubscriptionUc()
        {
            InitializeComponent();
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_WorkCompleted);

            SubscriptionCost.Text = subscriptionText;
        }
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.Visibility = Visibility.Visible));
            for (int i = 0; i < 345; i++)
            {
                Thread.Sleep(100);
                ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.Value += 1));
                if (!bgWorkerWorking)
                    break;
            }
        }

        private void bgWorker_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBar.Dispatcher.Invoke(new Action(() =>
            {
                ProgressBar.Visibility = Visibility.Hidden;
                ProgressBar.Value = 0;
            }));
        }
        private async void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            BtnShow.IsEnabled = false;
            await Account.Network.SendCallAsync(new bakSoftToHardCurrentRateRequestMessage());
            await Task.Delay(3000);
            if (Account.Game.bakRate != 0)
            {
                subscriptionText = (Math.Ceiling(Account.Game.bakRate * 800)).ToString();
                SubscriptionCost.Dispatcher.Invoke(new Action(() => SubscriptionCost.Text = subscriptionText));
                BtnBuy.IsEnabled = true;

                showTimeout = new System.Timers.Timer(); // Fix timer interval to elapsed if bought
                showTimeout.Interval = 60000;
                showTimeout.Elapsed += (s, en) =>
                {
                    SubscriptionCost.Dispatcher.Invoke(new Action(() =>
                    {
                        subscriptionText = "?";
                        SubscriptionCost.Dispatcher.Invoke(new Action(() => SubscriptionCost.Text = subscriptionText));
                        BtnShow.Dispatcher.Invoke(new Action(() => BtnShow.IsEnabled = true));
                        BtnBuy.Dispatcher.Invoke(new Action(() => BtnBuy.IsEnabled = false));
                    }));
                    showTimeout.Stop();
                };
                showTimeout.Start();

                return;
            }
            subscriptionText = "?";
            SubscriptionCost.Dispatcher.Invoke(new Action(() => SubscriptionCost.Text = subscriptionText));
            BtnShow.IsEnabled = true;
        }

        private void SetAlert(String message, int Interval = 3000, bool success = false)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = Interval;
            tbAlert.Dispatcher.Invoke(new Action(() => tbAlert.Text = message));
            if (success)
                spAlert.Dispatcher.Invoke(new Action(() => spAlert.Background = new SolidColorBrush(Color.FromArgb(255, 46, 176, 59))));
            else
                spAlert.Dispatcher.Invoke(new Action(() => spAlert.Background = new SolidColorBrush(Color.FromArgb(255, 205, 93, 93))));

            spAlert.Dispatcher.Invoke(new Action(() =>
            {
                spAlert.Visibility = Visibility.Visible;
            }));

            // above two line sets the visibility and shows the message and interval elapses hide the visibility of the label. Elapsed will we called after Start() method.

            timer.Elapsed += (s, en) =>
            {
                spAlert.Dispatcher.Invoke(new Action(() => spAlert.Visibility = Visibility.Hidden));
                timer.Stop(); // Stop the timer(otherwise keeps on calling)
            };
            timer.Start(); // Starts the timer. 
        }


        private async void Subscribe()
        {
            BtnBuy.Dispatcher.Invoke(new Action(() =>
            {
                BtnBuy.IsEnabled = false;
            }));

            bgWorkerWorking = true;
            bgWorker.RunWorkerAsync();

            await Account.Network.SendCallAsync(new restoreMysteryBoxMessage()).ConfigureAwait(false);
            await Account.Network.SendCallAsync(new setShopDetailsRequest()).ConfigureAwait(false);
            Thread.Sleep(1500);
            ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.Value = 12));
            Account.Logger.LogInfo(LanguageManager.Translate("606"), LanguageManager.Translate("621"));
            await Account.Network.SendCallAsync(new shopOpenRequest()).ConfigureAwait(false);
            Thread.Sleep(1500);
            ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.Value = 24));
            Account.Logger.LogInfo(LanguageManager.Translate("606"), LanguageManager.Translate("622"));
            await Account.Network.SendCallAsync(new shopOpenCategoryRequest(557, 1, 3)).ConfigureAwait(false);
            Thread.Sleep(1500);
            ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.Value = 36));
            Account.Logger.LogInfo(LanguageManager.Translate("606"), LanguageManager.Translate("623", subscriptionText));
            await Account.Network.SendCallAsync(new shopBuyRequest("KMS", 800, Convert.ToInt64(subscriptionText), 1, 7537, false)).ConfigureAwait(false);

            System.Threading.SpinWait.SpinUntil(() => (Account.Game.shopBuyInfo != 0), TimeSpan.FromSeconds(30));
            bgWorkerWorking = false;

            switch (Account.Game.shopBuyInfo)
            {
                case 0:
                    Account.Game.shopBuyInfo = 0;
                    SetAlert(LanguageManager.Translate("619"), 5000, false);
                    Account.Logger.LogError(LanguageManager.Translate("606"), LanguageManager.Translate("619"));
                    showTimeout.Interval = 1;
                    break;

                case 1:
                    Account.Game.shopBuyInfo = 0;
                    SetAlert(LanguageManager.Translate("618"), 20000, true);
                    Account.Logger.LogInfo(LanguageManager.Translate("606"), LanguageManager.Translate("618"));
                    showTimeout.Interval = 1;
                    break;
                case -1:
                    Account.Game.shopBuyInfo = 0;
                    SetAlert(LanguageManager.Translate("620"), 5000, false);
                    Account.Logger.LogError(LanguageManager.Translate("606"), LanguageManager.Translate("620"));
                    showTimeout.Interval = 1;
                    break;
            }
        }
        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (Account.State != Core.Enums.AccountStates.NONE)
            {
                SetAlert("Le personnage doit être en jeu et inactif");
                return;
            }
            if (Account.Game.Character.Inventory.Kamas < Convert.ToDouble(subscriptionText))
            {
                SetAlert("Vous n'avez pas assez de kamas pour acheter l'abonnement");
                return;
            }
            Subscribe();
        }

    }
}
