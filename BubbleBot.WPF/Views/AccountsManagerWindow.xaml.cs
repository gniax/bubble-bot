using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
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

namespace BubbleBot.Views
{
    public partial class AccountsManagerWindow
    {
        // Constructor
        public AccountsManagerWindow()
        {
            InitializeComponent();

            DataContext = GlobalConfiguration.Instance;
            TxtSeparator.Text = ":";

            CmbRace.ItemsSource = BreedsUtility.Breeds;
            CmbRace.SelectedIndex = 0;

            LoadConfigurations();
        }

        #region Configurations copier

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (LbAccountsCopier.SelectedItems.Count == 0)
                return;

            foreach (var account in LbAccountsCopier.SelectedItems.Cast<AccountConfiguration>())
            {
                if (CmbParametersCopier.SelectedIndex > 0)
                    File.Copy(
                        Path.Combine(Configuration.ConfigurationsPath, CmbParametersCopier.SelectedItem.ToString()),
                        Path.Combine(Configuration.ConfigurationsPath, account.Username + ".config"), true);

                if (CmbFightsConfigurationsCopier.SelectedIndex > 0)
                    File.Copy(
                        Path.Combine(FightsConfiguration.ConfigurationsPath,
                            CmbFightsConfigurationsCopier.SelectedItem.ToString()),
                        Path.Combine(FightsConfiguration.ConfigurationsPath, $"{account.Username}.fconfig"), true);

                Account connectedAccount = BubbleBotMain.Instance.ConnectedAccounts.Where(a => a.AccountConfig == account).FirstOrDefault();
                if (connectedAccount != null)
                {
                    connectedAccount.Configuration.Load();
                    connectedAccount.Extensions.Fights.Configuration.Load();
                }
                
            }
        }

        #endregion

        private void BtnSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            var i = Convert.ToInt32((sender as Button).Tag);
            var lb = i == 0 ? LbAccounts : LbAccountsCopier;
            lb.SelectAll();
        }

        private void BtnUnselectAll_OnClick(object sender, RoutedEventArgs e)
        {
            var i = Convert.ToInt32((sender as Button).Tag);
            var lb = i == 0 ? LbAccounts : LbAccountsCopier;
            lb.UnselectAll();
        }

        private void LoadConfigurations()
        {
            CmbParameters.Items.Add(LanguageManager.Translate("463"));
            CmbParametersCopier.Items.Add(LanguageManager.Translate("463"));
            if (Directory.Exists(Configuration.ConfigurationsPath))
                foreach (var file in Directory.GetFiles(Configuration.ConfigurationsPath, "*.config"))
                {
                    CmbParameters.Items.Add(Path.GetFileName(file));
                    CmbParametersCopier.Items.Add(Path.GetFileName(file));
                }

            CmbFightsConfigurations.Items.Add(LanguageManager.Translate("463"));
            CmbFightsConfigurationsCopier.Items.Add(LanguageManager.Translate("463"));
            if (Directory.Exists(FightsConfiguration.ConfigurationsPath))
                foreach (var file in Directory.GetFiles(FightsConfiguration.ConfigurationsPath, "*.fconfig"))
                {
                    CmbFightsConfigurations.Items.Add(Path.GetFileName(file));
                    CmbFightsConfigurationsCopier.Items.Add(Path.GetFileName(file));
                }

            CmbParameters.SelectedIndex = 0;
            CmbParametersCopier.SelectedIndex = 0;
            CmbFightsConfigurations.SelectedIndex = 0;
            CmbFightsConfigurationsCopier.SelectedIndex = 0;
        }


        #region Connect accounts

        private void BtnDeleteAccounts_Click(object sender, RoutedEventArgs e)
        {
            for (var i = LvAccounts.SelectedItems.Count - 1; i >= 0; i--)
                GlobalConfiguration.Instance.RemoveAccount(LvAccounts.SelectedItems[i] as AccountConfiguration);

            GlobalConfiguration.Instance.Save();
        }

        private void BtnConnectAccounts_Click(object sender, RoutedEventArgs e)
        {
            if (!BubbleBotMain.Instance.Server.LoggedIn)
                return;

            if (LvAccounts.SelectedItems.Count == 0)
                return;

            BubbleBotMain.Instance.Server.SendMessage(new ConnectAccountsRequestMessage(LvAccounts.SelectedItems
                .Cast<AccountConfiguration>().Select(a => a.Username).ToList()));
            Close();
        }

        private void BtnLoadAccounts_Click(object sender, RoutedEventArgs e)
        {
            if (!BubbleBotMain.Instance.Server.LoggedIn)
                return;

            if (LvAccounts.SelectedItems.Count == 0)
                return;

            BubbleBotMain.Instance.Server.SendMessage(new LoadAccountsRequestMessage(LvAccounts.SelectedItems
                .Cast<AccountConfiguration>().Select(a => a.Username).ToList()));
            Close();
        }

        private void LvAccounts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!BubbleBotMain.Instance.Server.LoggedIn)
                return;

            if (LvAccounts.SelectedItem == null)
                return;

            BubbleBotMain.Instance.Server.SendMessage(
                new LoadAccountRequestMessage((LvAccounts.SelectedItem as AccountConfiguration).Username));
            Close();
        }

        private void LvAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvAccounts.SelectedItems.Count > 1 && LvAccounts.SelectedItems.Count < 9)
            {
                GbGroup.Visibility = Visibility.Visible;
                CmbChief.SelectedIndex = 0;
            }
            else
            {
                GbGroup.Visibility = Visibility.Collapsed;
            }
        }

        private async void BtnLoadGroup_Click(object sender, RoutedEventArgs e)
        {
            if (!BubbleBotMain.Instance.Server.LoggedIn)
                return;

            if (LvAccounts.SelectedItems.Count < 2 || CmbChief.SelectedItem == null)
                return;

            var chief = CmbChief.SelectedItem as AccountConfiguration;
            var members = LvAccounts.SelectedItems.Cast<AccountConfiguration>().Where(a => a != chief).ToArray();

            // Check if all the accounts are in the same server
            if (!members.All(a => a.Server == chief.Server))
            {
                await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("392"));
                return;
            }

            BubbleBotMain.Instance.Server.SendMessage(
                new LoadGroupRequestMessage(new[] {chief.Username}.Concat(members.Select(m => m.Username)).ToList()));
            Close();
        }

        private async void BtnConnectGroup_Click(object sender, RoutedEventArgs e)
        {
            if (!BubbleBotMain.Instance.Server.LoggedIn)
                return;

            if (LvAccounts.SelectedItems.Count < 2 || CmbChief.SelectedItem == null)
                return;

            var chief = CmbChief.SelectedItem as AccountConfiguration;
            var members = LvAccounts.SelectedItems.Cast<AccountConfiguration>().Where(a => a != chief).ToArray();

            // Check if all the accounts are in the same server
            if (!members.All(a => a.Server == chief.Server))
            {
                await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("392"));
                return;
            }

            BubbleBotMain.Instance.Server.SendMessage(
                new ConnectGroupRequestMessage(new[] {chief.Username}.Concat(members.Select(m => m.Username))
                    .ToList()));
            Close();
        }

        #endregion

        #region Add accounts

        private async void BtnAddAccount_Click(object sender, RoutedEventArgs e)
        {
            if (TxtUsername.Text.Length < 3 || TxtPassword.Password.Length < 3)
            {
                await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("250"));
                return;
            }

            if (GlobalConfiguration.Instance.GetAccount(TxtUsername.Text) != null)
            {
                await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("398"));
                return;
            }

            GlobalConfiguration.Instance.AddAccountAndSave(TxtUsername.Text, TxtPassword.Password, CmbServer.Text,
                TxtCharacter.Text, TxtNickname.Text, TxtIdentifiant.Text, false);

            TxtUsername.Clear();
            TxtPassword.Clear();
            TxtCharacter.Clear();
            TxtIdentifiant.Clear();
            TxtNickname.Clear();
        }

        private void TxtSeparator_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TxtSeparatorDefaultPreview.Text = TxtSeparator.Text.Length == 0
                ? $"-"
                : $"{LanguageManager.Translate("495", TxtSeparator.Text)}";
        }
        private void ImportDefaultRdbtn_Click(object sender, RoutedEventArgs e)
        {
            TxtCustomImportFormat.Text = "...";
        }

        private void ImportCustomRdbtn_Click(object sender, RoutedEventArgs e)
        {
            var importDialog = new ImportFormatWindow(this);
            importDialog.ShowDialog();
        }

        private async void BtnImportAccounts_OnClick(object sender, RoutedEventArgs e)
        {
            if (RdbtnDefault.IsChecked == true)
            {
                if (TxtSeparator.Text.Length == 0 || TxtFilePath.Text.Length == 0 || !File.Exists(TxtFilePath.Text))
                {
                    await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("496"));
                    return;
                }

                var lines = File.ReadAllLines(TxtFilePath.Text);
                var accounts = new List<AccountConfiguration>();

                for (var i = 0; i < lines.Length; i++)
                {
                    var infos = lines[i].Split(new[] {TxtSeparator.Text}, StringSplitOptions.RemoveEmptyEntries);

                    if (infos.Length < 2)
                        continue;

                    var nbparameters = infos.Length;

                    if (nbparameters == 2)
                        accounts.Add(new AccountConfiguration(infos[0], infos[1], "-", "", "", "", false));

                    if (nbparameters == 3)
                        accounts.Add(new AccountConfiguration(infos[0], infos[1], "-", "", infos[2], "", false));

                    if (nbparameters == 5)
                    {
                        accounts.Add(new AccountConfiguration(infos[0], infos[1], "-", "", infos[2], "", false));
                        accounts.ElementAt(i).Proxy.Ip = infos[3];
                        ushort.TryParse(infos[4], out var paramport);
                        accounts.ElementAt(i).Proxy.Port = paramport;
                    }

                    if (nbparameters == 7)
                    {
                        accounts.Add(new AccountConfiguration(infos[0], infos[1], "-", "", infos[2], "", false));
                        accounts.ElementAt(i).Proxy.Ip = infos[3];
                        ushort.TryParse(infos[4], out var paramport);
                        accounts.ElementAt(i).Proxy.Port = paramport;
                        accounts.ElementAt(i).Proxy.Username = infos[5];
                        accounts.ElementAt(i).Proxy.Password = infos[6];
                    }
                }

                if (accounts.Count > 0)
                {
                    GlobalConfiguration.Instance.AddAccountsAndSave(accounts);
                    await this.ShowMessageAsync(LanguageManager.Translate("492"),
                        LanguageManager.Translate("497", accounts.Count));
                }
                else
                {
                    await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("498"));
                }
            }
            else if (RdbtnCustom.IsChecked == true)
            {
                if (TxtCustomImportFormat.Text != "...")
                {
                    if (TxtFilePath.Text.Length == 0 || !File.Exists(TxtFilePath.Text))
                    {
                        await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("496"));
                        return;
                    }

                    var lines = File.ReadAllLines(TxtFilePath.Text);
                    var accounts = new List<AccountConfiguration>();
                    short errors = 0;

                    for (var i = 0; i < lines.Length; i++)
                    {
                        string delimiter = null;
                        string pattern = @"^[a-zA-Z]+(\W)";
                        var regMatch = Regex.Match(TxtCustomImportFormat.Text, pattern);
                        if (regMatch.Success)
                        {
                            delimiter = regMatch.Groups[1].Value;
                        }

                        if (delimiter == null)
                            return;

                        var content = lines[i].Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                        var keys = TxtCustomImportFormat.Text.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var account = new AccountConfiguration("", "", "-", "", "", "", false);

                        for (int j = 0; j < keys.Count; j++)
                        {
                            switch (keys[j])
                            {
                                case "username":
                                    account.Username = content[j] != null ? content[j] : "";
                                    break;
                                case "password":
                                    account.Password = content[j] != null ? content[j] : "";
                                    break;
                                case "ip":
                                    account.Proxy.Ip = content[j] != null ? content[j] : "";
                                    break;
                                case "port":
                                    if (content[j] != null)
                                    {
                                        var isNumeric = UInt16.TryParse(content[j], out ushort value);
                                        account.Proxy.Port = isNumeric ? value : (ushort) 0;
                                    }
                                    break;
                                case "pxy-user":
                                    account.Proxy.Username = content[j] != null ? content[j] : "";
                                    break;
                                case "pxy-pass":
                                    account.Proxy.Password = content[j] != null ? content[j] : "";
                                    break;
                                case "id":
                                    account.Identifiant = content[j] != null ? content[j] : "";
                                    break;
                                case "groupe":
                                    account.Nickname = content[j] != null ? content[j] : "";
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (account.Username == "" && account.Password == "")
                        {
                            errors++;
                            break;
                        }

                        accounts.Add(account);
                        
                    }

                    if (accounts.Count > 0)
                    {
                        GlobalConfiguration.Instance.AddAccountsAndSave(accounts);
                        if (errors > 0)
                        {
                            await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("742", lines.Length, errors));
                        }
                        else
                        {
                            await this.ShowMessageAsync(LanguageManager.Translate("492"), LanguageManager.Translate("497", accounts.Count));
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("498"));
                    }
                }
                else
                {
                    await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("741"));
                }
            }
        }

        private void BtnSelectFilePath_OnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            var result = ofd.ShowDialog();

            if (!result.HasValue || !result.Value)
                return;

            TxtFilePath.Text = ofd.FileName;
        }

        private async void BtnImportAccountsIncr_OnClick(object sender, RoutedEventArgs e)
        {
            if (TxtUsernameIncr.Text.Length == 0 || TxtPasswordIncr.Password.Length == 0 ||
                NudEndIncr.Value.Value <= NudStartIncr.Value.Value)
            {
                await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("496"));
                return;
            }

            var start = (int) NudStartIncr.Value.Value;
            var end = (int) NudEndIncr.Value.Value;
            var accounts = new List<AccountConfiguration>();
            for (var i = start; i <= end; i++)
                accounts.Add(new AccountConfiguration($"{TxtUsernameIncr.Text}{i}", TxtPasswordIncr.Password, "-", "",
                    "", "", false));

            if (accounts.Count <= 0)
                return;

            GlobalConfiguration.Instance.AddAccountsAndSave(accounts);
            await this.ShowMessageAsync(LanguageManager.Translate("492"),
                LanguageManager.Translate("497", accounts.Count));
        }

        private void TxtUsernameIncr_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshIncrPreview();
        }

        private void NudIncr_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            RefreshIncrPreview();
        }

        private void RefreshIncrPreview()
        {
            if (TxtIncrPreview == null)
                return;

            TxtIncrPreview.Text = TxtUsernameIncr.Text.Length == 0
                ? "-"
                : LanguageManager.Translate("551", TxtUsernameIncr.Text, NudStartIncr.Value.Value,
                    NudEndIncr.Value.Value);
        }

        #endregion

        #region Characters creator

        //Lors de la création dans la liste de compte
        private void BtnCreateCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (LvAccounts.SelectedItems == null)
                return;
            Console.WriteLine(LvAccounts.SelectedItems.Count.ToString());
            var selectedAccounts = LvAccounts.SelectedItems.Cast<AccountConfiguration>().ToList();
            var accountCreatorInterface = new AccountsCharacterCreator(selectedAccounts);
            accountCreatorInterface.ShowDialog();
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
            return Color.FromRgb((byte) random.Next(256), (byte) random.Next(256), (byte) random.Next(256));
        }

        private async void CmbCompleteTutorial_OnChecked(object sender, RoutedEventArgs e)
        {
           // await this.ShowMessageAsync(LanguageManager.Translate("513"), LanguageManager.Translate("515"));
        }

        #endregion

        #region Proxies

        private async void BtnTestProxy_Click(object sender, RoutedEventArgs e)
        {
            if (!ushort.TryParse(TxtProxyPort.Text, out var port))
                return;

            if (!IPAddress.TryParse(TxtProxyIp.Text, out var ip))
                ip = Dns.GetHostEntry(TxtProxyIp.Text).AddressList[0];

            if (ip == null)
                return;

            using (var http = new HttpClient(new HttpClientHandler
                {
                    Proxy = new WebProxy($"http://{ip}:{port}", false)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(TxtProxyUsername.Text, TxtProxyPassword.Text)
                    },
                    PreAuthenticate = true,
                    UseDefaultCredentials = false
                })
                {Timeout = new TimeSpan(0, 0, 5)})
            {
                try
                {
                    var response = await http.GetAsync("https://ipv4.icanhazip.com/");
                    response.EnsureSuccessStatusCode();

                    var text = await response.Content.ReadAsStringAsync();

                    // If the ip is valid, the proxy is working
                    if (text.Substring(0, text.Length - 1) == ip.ToString())
                    {
                        response = await http.GetAsync("https://proxyconnection.touch.dofus.com/haapi/getForumPostsList?lang=fr&topicId=24993");
                        if ((int)response.StatusCode == 403)
                        {
                            await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("731"));
                        }
                        else if ((int)response.StatusCode == 200)
                        {
                            await this.ShowMessageAsync(LanguageManager.Translate("357"), LanguageManager.Translate("355"));
                        }
                        else
                        {
                            await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("732", response.StatusCode));
                        }
                        return;
                    }
                }
                catch
                {
                }

                await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("356"));
            }
        }

        private void BtnSaveProxy_Click(object sender, RoutedEventArgs e)
        {
            var ip = TxtProxyIp.Text.Length == 0 ? "" :
                IPAddress.TryParse(TxtProxyIp.Text, out var ipAddress) ? ipAddress.ToString() : null;
            if (ip == null || !ushort.TryParse(TxtProxyPort.Text, out var port))
                return;

            if (!(LbAccountsListProxy.SelectedItem is AccountConfiguration account))
                return;

            BubbleBotMain.Instance.Server.SendMessage(new SetProxyRequestMessage(account.Username, ip, port,
                TxtProxyUsername.Text, TxtProxyPassword.Text));
        }

        private void BtnResetProxy_Click(object sender, RoutedEventArgs e)
        {
            if (!(LbAccountsListProxy.SelectedItem is AccountConfiguration account))
                return;

            var selectedAccount = LbAccountsListProxy.SelectedItem as AccountConfiguration;

            if (selectedAccount.Proxy.Ip != "" || selectedAccount.Proxy.Port != 0 ||
                selectedAccount.Proxy.Username != "" || selectedAccount.Proxy.Password != "")
                BubbleBotMain.Instance.Server.SendMessage(new SetProxyRequestMessage(selectedAccount.Username, "", 0,
                    "", ""));
        }

        #endregion

        #region Accounts edit/sort/export

        private void BtnSortByPseudo(object sender, RoutedEventArgs e)
        {
            var selectAcc = new List<AccountConfiguration>();
            for (var i = LvAccounts.SelectedItems.Count - 1; i >= 0; i--)
                selectAcc.Add(LvAccounts.SelectedItems[i] as AccountConfiguration);

            GlobalConfiguration.Instance.SortAccountPseudo(selectAcc);
            GlobalConfiguration.Instance.Save();
        }

        private void BtnEditPseudo(object sender, RoutedEventArgs e)
        {
            var choiceInterface = new AccountsEditPseudo();
            choiceInterface.ShowDialog();
            var newPseudoSelected = choiceInterface.newPseudo;

            if (newPseudoSelected != "")
            {
                for (var i = LvAccounts.SelectedItems.Count - 1; i >= 0; i--)
                    GlobalConfiguration.Instance.SetAccountPseudo(LvAccounts.SelectedItems[i] as AccountConfiguration,
                        newPseudoSelected);

                GlobalConfiguration.Instance.Save();
            }
        }

        private void BtnEditAccount(object sender, RoutedEventArgs e)
        {
            if (LvAccounts.SelectedItem == null)
                return;

            var selectedAccount = LvAccounts.SelectedItem as AccountConfiguration;
            var accountInterface = new AccountsEditor(selectedAccount);
            accountInterface.ShowDialog();
        }

        private void ExportAccountTxt_Click(object sender, RoutedEventArgs e)
        {
            var selectAcc = new List<AccountConfiguration>();
            for (var i = LvAccounts.SelectedItems.Count - 1; i >= 0; i--)
                selectAcc.Add(LvAccounts.SelectedItems[i] as AccountConfiguration);

            if (selectAcc.Count == 0)
                return;

            var exportInterface = new ExportFormatWindow(selectAcc);
            exportInterface.ShowDialog();
        }


        #endregion


    }
}