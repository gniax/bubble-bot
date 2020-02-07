using MahApps.Metro.Controls.Dialogs;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Accounts.Extensions.Bid;
using BubbleBot.Protocol.Data;
using BubbleBot.Server.Messages;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using BubbleBot.Configurations.Language;
using BubbleBot.Protocol.Messages;
using System.Threading.Tasks;
using BubbleBot.Protocol.Messages.Messages;
using System.Threading;
using Newtonsoft.Json;
using System.Windows.Media;
using System.ComponentModel;

namespace BubbleBot.Views
{
    /// <summary>
    /// Logique d'interaction pour AccountsManagerEditAccount.xaml
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
            this.Close();
        }
    }
}

