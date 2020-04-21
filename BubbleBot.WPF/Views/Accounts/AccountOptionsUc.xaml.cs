using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Accounts.Configurations;
using BubbleBot.Protocol.Data;
using BubbleBot.Server.Messages;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace BubbleBot.Views.Accounts
{
    public partial class AccountOptionsUc : UserControl
    {
        // Constructor
        public AccountOptionsUc()
        {
            InitializeComponent();
        }

        // Properties
        private Account Account => BubbleBotMain.Instance.SelectedAccount;


        private void BtnDeleteSpellToBoost_Click(object sender, RoutedEventArgs e)
        {
            if (lvSpellsToBoost.SelectedItem == null)
                return;

            Account.Configuration.SpellsToBoost.Remove(lvSpellsToBoost.SelectedItem as SpellToBoostEntry);
            Account.Configuration.Save();
        }

        private async void BtnAddSpellToBoost_Click(object sender, RoutedEventArgs e)
        {
            var spell = DataManager.Get<Spells>((int) nudSpellId.Value);
            var window = Window.GetWindow(this) as MetroWindow;
            if (spell == null)
            {
                await window.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("372"));
                return;
            }

            if (Account.Configuration.SpellsToBoost.FirstOrDefault(s => s.Id == spell.Id) != null)
            {
                await window.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("373"));
                return;
            }

            Account.Configuration.SpellsToBoost.Add(new SpellToBoostEntry(spell.Id, spell.NameId,
                (byte) nudSpellLevel.Value));
            Account.Configuration.Save();
        }

        private void BtnDeleteAuthorizedTradeFrom_Click(object sender, RoutedEventArgs e)
        {
            if (lbAuthorizedTradesFrom.SelectedItem == null)
                return;

            Account.Configuration.AuthorizedTradesFrom.Remove((int) lbAuthorizedTradesFrom.SelectedItem);
            Account.Configuration.Save();
        }

        private async void BtnAddAuthorizedTradeFrom_Click(object sender, RoutedEventArgs e)
        {
            var characterId = (int) nudCharacterId.Value;

            if (Account.Configuration.AuthorizedTradesFrom.Contains(characterId))
            {
                var window = Window.GetWindow(this) as MetroWindow;
                await window.ShowMessageAsync(LanguageManager.Translate("249"), "Personnage déjà ajouté.");
                return;
            }

            BubbleBotMain.Instance.Server.SendMessage(
                new AddAuthorizedTradeFromRequestMessage(Account.AccountConfig.Username, characterId));
        }
    }
}