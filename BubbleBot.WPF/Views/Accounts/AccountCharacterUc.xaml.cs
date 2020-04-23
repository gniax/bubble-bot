using System;
using System.Windows;
using System.Windows.Controls;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Accounts.InGame.Character;
using BubbleBot.Core.Enums;

namespace BubbleBot.Views.Accounts
{
    public partial class AccountCharacterUc : UserControl
    {
        // Constructor
        public AccountCharacterUc()
        {
            InitializeComponent();
        }

        // Properties
        private Account Account => BubbleBotMain.Instance.SelectedAccount;


        private void BtnUpSpell_Click(object sender, RoutedEventArgs e)
        {
            var spell = (sender as Button).DataContext as SpellEntry;

            if (spell == null)
                return;

            Account.Game.Character.LevelUpSpell(spell);
        }

        private void BtnBoostStat_Click(object sender, RoutedEventArgs e)
        {
            var boostableStat = (BoostableStats) Convert.ToInt32((sender as Button).Tag);
            Account.Game.Character.BoostStat(boostableStat);
        }
    }
}