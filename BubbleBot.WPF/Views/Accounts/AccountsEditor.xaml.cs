using System.Windows;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Views.Accounts
{
    /// <summary>
    ///     Logique d'interaction pour AccountsEditor.xaml
    /// </summary>
    public partial class AccountsEditor
    {
        public AccountsEditor(AccountConfiguration account)
        {
            InitializeComponent();
            Account = account;

            Title = LanguageManager.Translate("636", Account.Username);
            txtPassword.Password = Account.Password;
            txtUsername.Text = Account.Username;
            cmbServer.SelectedIndex = getServerIndex(Account.Server);
            txtCharacter.Text = Account.Character;
            txtGroup.Text = Account.Identifiant;
            txtPseudo.Text = Account.Nickname;
        }

        private AccountConfiguration Account { get; }

        private void btnConfirmation_Click(object sender, RoutedEventArgs e)
        {
            if (Account.Username != txtUsername.Text)
                Account.State = 0;

            Account.Username = txtUsername.Text;
            Account.Password = txtPassword.Password;
            Account.Server = cmbServer.Text;
            Account.Character = txtCharacter.Text;
            Account.Nickname = txtGroup.Text;
            Account.Identifiant = txtPseudo.Text;
            GlobalConfiguration.Instance.Save();
            GlobalConfiguration.Instance.RaisePropertyChanged("AccountsList");
            Close();
        }

        private int getServerIndex(string serverName)
        {
            switch (serverName)
            {
                case "-": return 0;
                case "Terra Cogita": return 1;
                case "Herdegrize": return 2;
                case "Oshimo": return 3;
                case "Dodge": return 4;
                case "Brutas": return 5;
                case "Grandapan": return 6;
                default: return 0;
            }
        }
    }
}