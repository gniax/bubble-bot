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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Accounts.Configurations;
using BubbleBot.Core.Accounts.Extensions.Fights.Configuration;
using BubbleBot.Protocol.Data;
using BubbleBot.Server.Messages;
using BubbleBot.Utility.DofusTouch;
using BubbleBot.Views.Accounts;
using ColorPickerWPF;
using ColorPickerWPF.Code;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Path = System.IO.Path;
using System.Collections.ObjectModel;
using System.IO;

namespace BubbleBot.Views
{
    /// <summary>
    /// Logique d'interaction pour AccountsCharacterCreator.xaml
    /// </summary>
    public partial class AccountsCharacterCreator 
    {
       //public ObservableCollection<AccountConfiguration> SelectedAccountsList { get; set;}

        public AccountsCharacterCreator(List<AccountConfiguration> listOfSelectedItems)
        {
            InitializeComponent();
            LbAccounts.ItemsSource = listOfSelectedItems;
            CmbRace.ItemsSource = BreedsUtility.Breeds;
            CmbRace.SelectedIndex = 0;
            LoadConfigurations();
        }

        private void LoadConfigurations()
        {
            CmbParameters.Items.Add(LanguageManager.Translate("463"));

            if (Directory.Exists(Configuration.ConfigurationsPath))
                foreach (var file in Directory.GetFiles(Configuration.ConfigurationsPath, "*.config"))
                {
                    CmbParameters.Items.Add(Path.GetFileName(file));
                }

            CmbFightsConfigurations.Items.Add(LanguageManager.Translate("463"));
            if (Directory.Exists(FightsConfiguration.ConfigurationsPath))
                foreach (var file in Directory.GetFiles(FightsConfiguration.ConfigurationsPath, "*.fconfig"))
                {
                    CmbFightsConfigurations.Items.Add(Path.GetFileName(file));
                }

            CmbParameters.SelectedIndex = 0;
            CmbFightsConfigurations.SelectedIndex = 0;
        }

        private void BtnSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            var i = Convert.ToInt32((sender as Button).Tag);
            var lb = LbAccounts;
            lb.SelectAll();
        }

        private void BtnUnselectAll_OnClick(object sender, RoutedEventArgs e)
        {
           // var i = Convert.ToInt32((sender as Button).Tag);
            var lb = LbAccounts;
            lb.UnselectAll();
        }

        private void CmbRace_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshBreedInfos();
        }

        private void CmbSex_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshBreedInfos();
        }

        private void ColorRects_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var rect = sender as Rectangle;
            Color color;
            if (ColorPickerWindow.ShowDialog(out color, ColorPickerDialogOptions.SimpleView))
                rect.Fill = new SolidColorBrush(color);
        }

        private void BtnRandomColors_OnClick(object sender, RoutedEventArgs e)
        {
            var breed = CmbRace.SelectedItem as Breeds;

            if (breed == null)
                return;

            RectColor1.Fill = new SolidColorBrush(GetRandomColor());
            RectColor2.Fill = new SolidColorBrush(GetRandomColor());
            RectColor3.Fill = new SolidColorBrush(GetRandomColor());
            RectColor4.Fill = new SolidColorBrush(GetRandomColor());
            RectColor5.Fill = new SolidColorBrush(GetRandomColor());
        }

        private void BtnRefreshColors_OnClick(object sender, RoutedEventArgs e)
        {
            var breed = CmbRace.SelectedItem as Breeds;

            if (breed == null)
                return;

            SetBreedBaseColors(breed);
        }


        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (LbAccounts.SelectedItems.Count == 0)
                return;

            foreach (AccountConfiguration account in LbAccounts.SelectedItems)
                account.CharacterCreation = GetCharacterCreation();

            GlobalConfiguration.Instance.Save();
        }

        private void BtnSaveAndConnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (LbAccounts.SelectedItems.Count == 0)
                return;

            BtnSave_OnClick(null, null);
            BubbleBotMain.Instance.Server.SendMessage(new ConnectAccountsRequestMessage(LbAccounts.SelectedItems
                .Cast<AccountConfiguration>().Select(a => a.Username).ToList()));
            Close();
        }

        private CharacterCreation GetCharacterCreation()
        {
            if (!CbCreateCharacter.IsChecked.Value)
                return new CharacterCreation();

            return new CharacterCreation
            {
                Create = true,
                Name = TxtName.Text,
                Server = CmbServerCC.Text,
                Breed = CbRandomBreed.IsChecked.Value ? -1 : (CmbRace.SelectedItem as Breeds).Id,
                Sex = CbRandomSex.IsChecked.Value ? -1 : CmbSex.SelectedIndex,
                Head = CbRandomHead.IsChecked.Value ? -1 : CmbHead.SelectedIndex,
                Colors = CbRandomHead.IsChecked.Value
                    ? new List<int>(5)
                    {
                        BreedsUtility.GetIndexedColor(1, GetRandomColor()),
                        BreedsUtility.GetIndexedColor(2, GetRandomColor()),
                        BreedsUtility.GetIndexedColor(3, GetRandomColor()),
                        BreedsUtility.GetIndexedColor(4, GetRandomColor()),
                        BreedsUtility.GetIndexedColor(5, GetRandomColor())
                    }
                    : new List<int>(5)
                    {
                        BreedsUtility.GetIndexedColor(1, (RectColor1.Fill as SolidColorBrush).Color),
                        BreedsUtility.GetIndexedColor(2, (RectColor2.Fill as SolidColorBrush).Color),
                        BreedsUtility.GetIndexedColor(3, (RectColor3.Fill as SolidColorBrush).Color),
                        BreedsUtility.GetIndexedColor(4, (RectColor4.Fill as SolidColorBrush).Color),
                        BreedsUtility.GetIndexedColor(5, (RectColor5.Fill as SolidColorBrush).Color)
                    },
                ParametersToCopy = CmbParameters.SelectedIndex == 0 ? "" : CmbParameters.Text,
                FightsConfigurationToCopy =
                    CmbFightsConfigurations.SelectedIndex == 0 ? "" : CmbFightsConfigurations.Text,
                CompleteTutorial = CmbCompleteTutorial.IsChecked.Value
            };
        }

        private void RefreshBreedInfos()
        {
            var breed = CmbRace.SelectedItem as Breeds;

            if (breed == null)
                return;

            // Heads
            CmbHead.ItemsSource = BreedsUtility.GetBreedHeads(breed.Id, CmbSex.SelectedIndex);
            CmbHead.SelectedIndex = 0;

            // Colors
            SetBreedBaseColors(breed);
        }

        private void SetBreedBaseColors(Breeds breed)
        {
            var baseColors = BreedsUtility.GetBreedBaseColors(breed, CmbSex.SelectedIndex);
            RectColor1.Fill = new SolidColorBrush(baseColors[0]);
            RectColor2.Fill = new SolidColorBrush(baseColors[1]);
            RectColor3.Fill = new SolidColorBrush(baseColors[2]);
            RectColor4.Fill = new SolidColorBrush(baseColors[3]);
            RectColor5.Fill = new SolidColorBrush(baseColors[4]);
        }

        private static readonly Random random = new Random();

        private static Color GetRandomColor()
        {
            return Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        }

        private async void CmbCompleteTutorial_OnChecked(object sender, RoutedEventArgs e)
        {
           // await this.ShowMessageAsync(LanguageManager.Translate("513"), LanguageManager.Translate("515"));
        }


    }
}
