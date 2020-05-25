using System.Windows;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Views
{
    public partial class NicknameWindow
    {
        private Account _account;
        public string AccountName { get; set; }

        public NicknameWindow(Account account)
        {
            InitializeComponent();
            DataContext = this;
            _account = account;
            AccountName = LanguageManager.Translate("601", _account.AccountConfig.Username);
        }


        private void SetAlert(string text, Visibility visibility)
        {
            tbAlert.Text = text;
            spAlert.Visibility = visibility;
        }
        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (txtNickname.Text != null && txtNickname.Text != "")
            SetAlert("", Visibility.Hidden);

            await _account.Network.SendMessageAsync(new NicknameChoiceRequestMessage(txtNickname.Text));
            _account.Logger.LogInfo(LanguageManager.Translate("85"),
                LanguageManager.Translate("615", txtNickname.Text));
            Close();
        }
    }
}