using System.Windows;

namespace BubbleBot.Views.Accounts
{
    /// <summary>
    ///     Logique d'interaction pour AccountsManagerEditAccount.xaml
    /// </summary>
    public partial class AccountsEditPseudo
    {
        public string newPseudo = "";

        public AccountsEditPseudo()
        {
            InitializeComponent();
        }

        private void btn_pseudoValidation(object sender, RoutedEventArgs e)
        {
            newPseudo = txtNickname.Text;
            Close();
        }
    }
}