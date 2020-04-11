using BubbleBot.Core.Accounts;
using BubbleBot.Core.Enums;
using BubbleBot.Server.Enums;
using BubbleBot.Server.Messages;
using Microsoft.Win32;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BubbleBot.Configurations.Language;
using ExtensionsEnum = BubbleBot.Protocol.Server.Enums.Extensions;

namespace BubbleBot.Views.Accounts
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public partial class AccountUc : UserControl
    {

        // Properties
        private Account Account => BubbleBotMain.Instance.SelectedAccount;


        // Constructor
        public AccountUc()
        {
            InitializeComponent();

            DataContextChanged += AccountUc_DataContextChanged;
            tcMain.SelectionChanged += TcMain_SelectionChanged;

            Task.Run(async () =>
            {
                await Task.Delay(2000);
                LoadFunctionalities();
            });
        }


        private void AccountUc_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            tcMain.SelectedIndex = 0;
        }

        private async void ConnectDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // If the bot is connect, disconnect it
                if (Account.Network.Connected)
                {
                    await Account.Network.Disconnect("CLIENT_CLOSING");
                    //Account.PlanificationTimer.Stop();
                }
                // Otherwise connect it
                else if (Account.State == AccountStates.DISCONNECTED)
                {
                    await Account.Connect();
                }
            }
            catch { }
        }

        private void LoadScript_Click(object sender, object e)
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "Lua file (.lua) | *.lua";

                var result = ofd.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    BubbleBotMain.Instance.Server.SendMessage(new LoadScriptRequestMessage(Account.AccountConfig.Username, ofd.FileName, File.ReadAllText(ofd.FileName)));
                }
            }
            catch (Exception ex)
            {
                Account.Logger.LogError(LanguageManager.Translate("165"), ex.ToString());
            }
        }

        private void StartScript_Click(object sender, RoutedEventArgs e)
        {
            BubbleBotMain.Instance.Server.SendMessage(new StartScriptRequestMessage(Account.AccountConfig.Username));
        }

        private void StopScript_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Account.Scripts.StopScript();
            }
            catch (Exception ex)
            {
                Account.Logger.LogError("", ex.ToString());
            }
        }

        static bool removing = false;
        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (!removing)
            {
                if (BubbleBotMain.Instance.SelectedAccount != null && tcMain.SelectedIndex != null || tcMain.SelectedIndex <= -1)
                    await BubbleBotMain.Instance.RemoveSelectedAccount();
                removing = false;
            }
        }

        #region Functionalities

        private void LoadFunctionalities()
        {
            if (!BubbleBotMain.Instance.Server.LoggedIn || !BubbleBotMain.Instance.Server.IsSubscribedToTouch)
                return;

            BubbleBotMain.Instance.Server.RegisterMessage<ShowFunctionalityMessage>(HandleShowFunctionalityMessage);
            BubbleBotMain.Instance.Server.SendMessage(new ShowFunctionalitiesRequestMessage());
        }

        private void HandleShowFunctionalityMessage(ShowFunctionalityMessage message)
        {
            switch (message.Functionality)
            {
                case Functionalities.STATISTICS:
                    Application.Current.Dispatcher.Invoke(() => AddUserControlToTab(new AccountStatisticsUc(), 8));
                    break;
                case Functionalities.HDV:
                    Application.Current.Dispatcher.Invoke(() => AddUserControlToTab(new AccountBidUc(), 6));
                    break;
                case Functionalities.FLOOD:
                    Application.Current.Dispatcher.Invoke(() => AddUserControlToTab(new AccountFloodUc(), 4));
                    break;
            }
        }

        private void TcMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                switch (tcMain.SelectedIndex)
                {
                    // Flood & Statistics only need IsSubscribedToTouch
                    case 4:
                    case 8:
                        if (!BubbleBotMain.Instance.Server.IsSubscribedToTouch && !((tcMain.Items[tcMain.SelectedIndex] as TabItem).Content is TextBlock))
                        {
                            SetUnauthorizedTabText(tcMain.SelectedIndex);
                        }
                        break;
                    // Bid needs IsSubscribedToTouch & HasExtension
                    case 6:
                        if ((!BubbleBotMain.Instance.Server.IsSubscribedToTouch || !BubbleBotMain.Instance.Server.HasExtension(ExtensionsEnum.HDV))
                            && !((tcMain.Items[tcMain.SelectedIndex] as TabItem).Content is TextBlock))
                        {
                            SetUnauthorizedTabText(tcMain.SelectedIndex);
                        }
                        break;
                }
            }
            catch { }
        }

        private void AddUserControlToTab(UserControl uc, int tabIndex)
        {
            uc.SetBinding(DataContextProperty, new Binding("SelectedAccount")
            {
                Source = BubbleBotMain.Instance
            });
            (tcMain.Items[tabIndex] as TabItem).Content = uc;
        }

        private void SetUnauthorizedTabText(int index)
        {
            (tcMain.Items[index] as TabItem).Content = new TextBlock()
            {
                Text = LanguageManager.Translate("406"),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 16
            };
        }

        #endregion

    }
}
