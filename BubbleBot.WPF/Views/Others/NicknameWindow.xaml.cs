using System.Windows;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Views
{
    public partial class NicknameWindow
    {
        public string nickname;

        public NicknameWindow(Account account)
        {
            InitializeComponent();
            DataContext = this;
            _account = account;
            AccountName = LanguageManager.Translate("601", _account.AccountConfig.Username);
            Information = string.Format(LanguageManager.Translate("603"));
        }

        public Account _account { get; set; }
        public string AccountName { get; set; }
        public string Information { get; set; }

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

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            SetAlert("", Visibility.Hidden);
            nickname = txtNickname.Text;
            Close();
        }
    }
}