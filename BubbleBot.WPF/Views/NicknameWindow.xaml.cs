using BubbleBot.Server;
using BubbleBot.Server.Messages;
using System.Windows;
using BubbleBot.Server.Enums;
using BubbleBot.WPF.Views;
using BubbleBot.Configurations;
using System.Windows.Controls;
using BubbleBot.Configurations.Language;
using BubbleBot.Protocol.Messages;
using BubbleBot.Core.Accounts;

namespace BubbleBot.Views
{
    public partial class NicknameWindow
    {
        public Account _account { get; set; }
        public string AccountName { get; set; }
        public string Information { get; set; }
        public string nickname = null;
        public NicknameWindow(Account account)
        {
            InitializeComponent();
            DataContext = this;
            _account = account;
            AccountName = LanguageManager.Translate("601", _account.AccountConfig.Username);
            Information = string.Format(LanguageManager.Translate("603"));

        }
        private void SetAlert(string text, Visibility visibility)
        {
            tbAlert.Text = text;
            spAlert.Visibility = visibility;
        }

        private void HandleNicknameRefusedMessage(NicknameRefusedMessage message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SetAlert(LanguageManager.Translate("604"), Visibility.Visible);
                btnConnect.IsEnabled = true;
                txtNickname.IsEnabled = true;
            });
        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            SetAlert("", Visibility.Hidden);
            nickname = txtNickname.Text;
            this.Close();
        }
    }
}