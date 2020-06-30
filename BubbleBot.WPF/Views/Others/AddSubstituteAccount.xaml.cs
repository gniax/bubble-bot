using BubbleBot.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BubbleBot.Views
{
    public partial class AddSubstituteAccount
    {
        public AccountsManagerWindow WParent { get; set; }
        public AddSubstituteAccount(AccountsManagerWindow parent)
        {
            InitializeComponent();

            WParent = parent;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (TxtUsername.Text == null || TxtUsername.Text == "" || TxtUsername.Text == " ")
                return;

            if (TxtPassword.Text == null || TxtPassword.Text == "" || TxtPassword.Text == " ")
                return;

            if (TxtUsername.Text.Contains(" "))
                TxtUsername.Text = TxtUsername.Text.Replace(" ", "");

            if (TxtPassword.Text.Contains(" "))
                TxtPassword.Text = TxtPassword.Text.Replace(" ", "");

            GlobalConfiguration.Instance.AddAccountAndSave(TxtUsername.Text, TxtPassword.Text);
            WParent?.UpdateAvailableSubstituteAccounts();
            Close();
        }
    }
}
