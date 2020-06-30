using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
using MahApps.Metro.Controls;
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

            TempActivateReplacement = false;
            TempConnectAfterReplacement = false;
            TempStartScript = false;
            TempCreateCharacter = false;
            TempDisconnectTimer = 0;

            DataContext = GlobalConfiguration.Instance;

            int availableAccounts = GlobalConfiguration.Instance.SubstituteAccounts == null ? 0 : GlobalConfiguration.Instance.SubstituteAccounts.Count;
            LabelAvailableAccounts.Inlines.Add(new Bold(new Run($"({availableAccounts}) ")));
            LabelAvailableAccounts.Inlines.Add(new Run(LanguageManager.Translate("763")));

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

                Account connectedAccount = BubbleBotMain.Instance.EveryConnectedAccount().Where(a => a.AccountConfig == account).FirstOrDefault();
                if (connectedAccount != null)
                {
                    connectedAccount.Configuration.Load();
                    connectedAccount.Extensions.Fights.Configuration.Load();
                }
                
            }
        }

        #endregion

        #region Connect accounts

        private void BtnDeleteAccounts_Click(object sender, RoutedEventArgs e)
        {
            List<AccountConfiguration> list = new List<AccountConfiguration>();
            foreach (AccountConfiguration account in LvAccounts.SelectedItems)
            {
                list.Add(account);
            }

            foreach (var account in list)
            {
                GlobalConfiguration.Instance.RemoveAccount(account);
            }

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

        }

        private void BtnLoadAccounts_Click(object sender, RoutedEventArgs e)
        {
            if (!BubbleBotMain.Instance.Server.LoggedIn)
                return;

            if (LvAccounts.SelectedItems.Count == 0)
                return;

            BubbleBotMain.Instance.Server.SendMessage(new LoadAccountsRequestMessage(LvAccounts.SelectedItems
                .Cast<AccountConfiguration>().Select(a => a.Username).ToList()));

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
                GbGroup.IsEnabled = true;
                CmbChief.SelectedIndex = 0;
            }
            else
            {
                GbGroup.IsEnabled = false;
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
                TxtCharacter.Text, TxtNickname.Text, TxtIdentifiant.Text, 0);

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
                        accounts.Add(new AccountConfiguration(infos[0], infos[1], "-", "", "", "", 0));

                    if (nbparameters == 3)
                        accounts.Add(new AccountConfiguration(infos[0], infos[1], "-", "", infos[2], "", 0));

                    if (nbparameters == 4)
                        accounts.Add(new AccountConfiguration(infos[0], infos[1], "-", "", infos[2], infos[3], 0));

                    if (nbparameters == 6)
                    {
                        accounts.Add(new AccountConfiguration(infos[0], infos[1], "-", "", infos[2], infos[3], 0));
                        accounts.ElementAt(i).Proxy.Ip = infos[4];
                        ushort.TryParse(infos[5], out var paramport);
                        accounts.ElementAt(i).Proxy.Port = paramport;
                    }

                    if (nbparameters == 8)
                    {
                        accounts.Add(new AccountConfiguration(infos[0], infos[1], "-", "", infos[2], infos[3], 0));
                        accounts.ElementAt(i).Proxy.Ip = infos[4];
                        ushort.TryParse(infos[5], out var paramport);
                        accounts.ElementAt(i).Proxy.Port = paramport;
                        accounts.ElementAt(i).Proxy.Username = infos[6];
                        accounts.ElementAt(i).Proxy.Password = infos[7];
                    }

                    if (nbparameters == 9)
                    {
                        string serveurChar = infos[8];
                        if(serveurChar == "Terra Cogita" || serveurChar == "Herdegrize" || serveurChar == "Oshimo" || serveurChar == "Dodge" || serveurChar == "Brutas" || serveurChar == "Grandapan")
                            accounts.Add(new AccountConfiguration(infos[0], infos[1], serveurChar, "", infos[2], infos[3], 0));
                        else
                            accounts.Add(new AccountConfiguration(infos[0], infos[1], "-", "", infos[2], infos[3], 0));

                        accounts.ElementAt(i).Proxy.Ip = infos[4];
                        ushort.TryParse(infos[5], out var paramport);
                        accounts.ElementAt(i).Proxy.Port = paramport;
                        accounts.ElementAt(i).Proxy.Username = infos[6];
                        accounts.ElementAt(i).Proxy.Password = infos[7];
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
                        var account = new AccountConfiguration("", "", "-", "", "", "", 0);

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
                    "", "", 0));

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

            var selectedAccounts = LvAccounts.SelectedItems.Cast<AccountConfiguration>().ToList();
            var accountCreatorInterface = new AccountsCharacterCreator(selectedAccounts, this);
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
            {
                account.CharacterCreation = GetCharacterCreation();
            }

            LbAccounts.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
            BtnReset.Visibility = Visibility.Visible;

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

        private void BtnSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            var i = Convert.ToInt32((sender as Button).Tag);
            var lb = i == 0 ? LbAccounts : i == 1 ? LbAccountsCopier : LbAccountsListReplacement;
            lb.SelectAll();
        }

        private void BtnUnselectAll_OnClick(object sender, RoutedEventArgs e)
        {
            var i = Convert.ToInt32((sender as Button).Tag);
            var lb = i == 0 ? LbAccounts : i == 1 ? LbAccountsCopier : LbAccountsListReplacement;
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

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (LvAccounts.SelectedItems == null || LvAccounts.SelectedItems?.Count == 0)
                return;

            foreach (AccountConfiguration account in LbAccounts.SelectedItems)
            {
                account.CharacterCreation.Create = false;
            }

            LbAccounts.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
            BtnReset.Visibility = Visibility.Hidden;
        }

        private void LbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvAccounts.SelectedItems == null || LvAccounts.SelectedItems?.Count == 0)
            {
                BtnReset.Visibility = Visibility.Hidden;
                return;
            }

            var accounts = new List<AccountConfiguration>();
            foreach (AccountConfiguration account in LbAccounts.SelectedItems)
            {
                accounts.Add(account);
            }

            if (accounts.Any(a => a.CharacterCreation.Create))
            {
                BtnReset.Visibility = Visibility.Visible;
                return;
            }

            BtnReset.Visibility = Visibility.Hidden;
        }

        #region Accounts Replacement

        public bool TempActivateReplacement { get; set; }
        public bool TempConnectAfterReplacement { get; set; }
        public bool TempStartScript { get; set; }
        public int TempDisconnectTimer { get; set; }
        public bool TempCreateCharacter { get; set; }

        private void BtnSaveMultipleAccountsReplacement_Click(object sender, RoutedEventArgs e)
        {
            if (LbAccountsListReplacement.SelectedItems == null || LbAccountsListReplacement.SelectedItems?.Count <= 0 || GlobalConfiguration.Instance.SubstituteAccounts?.Count <= 0)
            {
                this.ShowMessageAsync(LanguageManager.Translate("757"), LanguageManager.Translate("770"));
                return;
            }

            short count = 0;
            for (int i = LbAccountsListReplacement.SelectedItems.Count - 1; i >= 0; i--)
            {
                var toBeReplace = LbAccountsListReplacement.SelectedItems[i] as AccountConfiguration;
                var willReplace = GlobalConfiguration.Instance.SubstituteAccounts.FirstOrDefault();
                if (willReplace == null)
                {
                    this.ShowMessageAsync(LanguageManager.Translate("757"), LanguageManager.Translate("771", count));
                    BtnSaveMultipleAccountsReplacement.IsEnabled = false;
                    break;
                }


                count++;
                toBeReplace.Username = willReplace.Username;
                toBeReplace.Password = willReplace.Password;
                toBeReplace.State = 0;
                GlobalConfiguration.Instance.RemoveAccount(willReplace);

                if (BubbleBotMain.Instance.MainWindow != null)
                {
                    BubbleBotMain.Instance.MainWindow.treeView.ItemsSource = BubbleBotMain.Instance.Entities;
                    BubbleBotMain.Instance.MainWindow.treeView.Items.Refresh();
                }
            }

            LbAccountsListReplacement.ItemsSource = GlobalConfiguration.Instance.AccountsReplacementList;
            LbAccountsListReplacement.Items.Refresh();
            LvAccountsReplacement.Items.Refresh();
            UpdateAvailableSubstituteAccounts();
            BtnSaveMultipleAccountsReplacement.IsEnabled = false;
            GlobalConfiguration.Instance.Save();
            UpdateAccountsUI();
        }

        private void BtnAddReplacement_Click(object sender, RoutedEventArgs e)
        {
            var asa = new AddSubstituteAccount(this);
            asa.ShowDialog();
        }

        private void BtnAddReplacements_Click(object sender, RoutedEventArgs e)
        {
            var asa = new AddSubstituteAccounts(this);
            asa.ShowDialog();
        }

        private void BtnRemoveReplacement_Click(object sender, RoutedEventArgs e)
        {
            for (int i=LvAccountsReplacement.SelectedItems.Count-1; i>=0; i--)
            {
                var account = LvAccountsReplacement.SelectedItems[i] as SubstituteAccount;
                GlobalConfiguration.Instance.RemoveAccount(account);
            }

            UpdateAvailableSubstituteAccounts();

            BtnRemoveReplacement.IsEnabled = false;
        }

        private void BtnRemoveSubstitute_Click(object sender, RoutedEventArgs e)
        {
            var obj = (sender as Button).DataContext as SubstituteAccount;
            GlobalConfiguration.Instance.RemoveAccount(obj);
            UpdateAvailableSubstituteAccounts();
        }

        private void LvAccountsReplacement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvAccountsReplacement.SelectedItems?.Count > 0)
            {
                BtnRemoveReplacement.IsEnabled = true;
            }
            else
            {
                BtnRemoveReplacement.IsEnabled = false;
            }
        }
        public void LbAccountsListReplacement_SelectionChangedCustom(object sender, SelectionChangedEventArgs e, bool fromCharacterCreator = false)
        {
            if (LbAccountsListReplacement.SelectedItems?.Count > 0 && ManualReplacementTab.IsSelected)
            {
                BtnSaveMultipleAccountsReplacement.IsEnabled = true;
            }
            else if (ManualReplacementTab.IsSelected)
            {
                BtnSaveMultipleAccountsReplacement.IsEnabled = false;
            }
            else if (AutoReplacementTab.IsSelected && (LbAccountsListReplacement.SelectedItems == null || LbAccountsListReplacement.SelectedItems?.Count == 0 || LbAccountsListReplacement.SelectedItems?.Count > 1))
            {
                if (!fromCharacterCreator)
                {
                    TempActivateReplacement = false;
                    TempConnectAfterReplacement = false;
                    TempStartScript = false;
                    TempCreateCharacter = false;
                    TempDisconnectTimer = 0;

                    Binding tempActivateReplacement = new Binding("TempActivateReplacement");
                    tempActivateReplacement.Source = this;
                    Binding tempConnectAfterReplacement = new Binding("TempConnectAfterReplacement");
                    tempConnectAfterReplacement.Source = this;
                    Binding tempStartScript = new Binding("TempStartScript");
                    tempStartScript.Source = this;
                    Binding tempCreateCharacter = new Binding("TempCreateCharacter");
                    tempCreateCharacter.Source = this;
                    Binding tempDisconnectTimer = new Binding("TempDisconnectTimer");
                    tempDisconnectTimer.Source = this;

                    BindingOperations.SetBinding(CbActivateReplacement, CheckBox.IsCheckedProperty, tempActivateReplacement);
                    BindingOperations.SetBinding(CbConnectAfterReplacement, CheckBox.IsCheckedProperty, tempConnectAfterReplacement);
                    BindingOperations.SetBinding(CbStartScript, CheckBox.IsCheckedProperty, tempStartScript);
                    BindingOperations.SetBinding(CbCreateCharacter, CheckBox.IsCheckedProperty, tempCreateCharacter);
                    BindingOperations.SetBinding(DisconnectTimer, NumericUpDown.ValueProperty, tempDisconnectTimer);
                }

                if (LbAccountsListReplacement.SelectedItems?.Count > 1)
                {
                    InfoReplacementSettings.Visibility = Visibility.Collapsed;
                    BtnApplyReplacementSettings.Visibility = Visibility.Visible;
                    // If they all planned to create the same character and it is activated, then we can display it
                    var accounts = LbAccountsListReplacement.SelectedItems.Cast<AccountConfiguration>().ToList();



                    if (accounts.Select(a => a.ReplacementConfiguration.SubstituteCharacterCreation.Breed).Distinct().Count() == 1 && 
                            accounts.Select(a => a.ReplacementConfiguration.SubstituteCharacterCreation.Head).Distinct().Count() == 1 && accounts.Select(a => a.ReplacementConfiguration.SubstituteCharacterCreation.Sex).Distinct().Count() == 1)
                    {
                        var account = accounts.First();
                        if (account.ReplacementConfiguration.SubstituteCharacterCreation.Breed == -1 || account.ReplacementConfiguration.SubstituteCharacterCreation.Sex == -1 || account.ReplacementConfiguration.SubstituteCharacterCreation.Head == -1)
                        {
                            ImgCharacterPreview.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/random.png"));
                            return;
                        }
                        else if (account.ReplacementConfiguration.SubstituteCharacterCreation.Breed == 0)
                        {
                            ImgCharacterPreview.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/inactive.png"));
                            return;
                        }
                        else
                        {
                            ImgCharacterPreview.Source = new BitmapImage(new Uri($"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/cosmetics/{account.ReplacementConfiguration.SubstituteCharacterCreation.Breed}{account.ReplacementConfiguration.SubstituteCharacterCreation.Sex}_{account.ReplacementConfiguration.SubstituteCharacterCreation.Head + 1}.png"));
                            return;
                        }
                    }
                }
                else
                {
                    BtnApplyReplacementSettings.Visibility = Visibility.Collapsed;
                    InfoReplacementSettings.Visibility = Visibility.Visible;
                }

                ImgCharacterPreview.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/inactive.png"));
            }
            else if (LbAccountsListReplacement.SelectedItems?.Count == 1 && AutoReplacementTab.IsSelected)
            {
                BtnApplyReplacementSettings.Visibility = Visibility.Collapsed;
                InfoReplacementSettings.Visibility = Visibility.Visible;

                var account = LbAccountsListReplacement.SelectedItem as AccountConfiguration;

                Binding tempActivateReplacement = new Binding("ActivateReplacement");
                tempActivateReplacement.Source = account.ReplacementConfiguration;
                Binding tempConnectAfterReplacement = new Binding("ConnectAfterReplacement");
                tempConnectAfterReplacement.Source = account.ReplacementConfiguration;
                Binding tempStartScript = new Binding("StartScript");
                tempStartScript.Source = account.ReplacementConfiguration;
                Binding tempCreateCharacter = new Binding("Create");
                tempCreateCharacter.Source = account.ReplacementConfiguration.SubstituteCharacterCreation;
                Binding tempDisconnectTimer = new Binding("DisconnectTimer");
                tempDisconnectTimer.Source = account.ReplacementConfiguration;

                BindingOperations.SetBinding(CbActivateReplacement, CheckBox.IsCheckedProperty, tempActivateReplacement);
                BindingOperations.SetBinding(CbConnectAfterReplacement, CheckBox.IsCheckedProperty, tempConnectAfterReplacement);
                BindingOperations.SetBinding(CbStartScript, CheckBox.IsCheckedProperty, tempStartScript);
                BindingOperations.SetBinding(CbCreateCharacter, CheckBox.IsCheckedProperty, tempCreateCharacter);
                BindingOperations.SetBinding(DisconnectTimer, NumericUpDown.ValueProperty, tempDisconnectTimer);

                if (account.ReplacementConfiguration.SubstituteCharacterCreation.Breed == -1 || account.ReplacementConfiguration.SubstituteCharacterCreation.Sex == -1 || account.ReplacementConfiguration.SubstituteCharacterCreation.Head == -1)
                {
                    ImgCharacterPreview.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/random.png"));
                }
                else if (account.ReplacementConfiguration.SubstituteCharacterCreation.Breed == 0)
                {
                    ImgCharacterPreview.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/inactive.png"));
                }
                else
                {
                    ImgCharacterPreview.Source = new BitmapImage(new Uri($"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/cosmetics/{account.ReplacementConfiguration.SubstituteCharacterCreation.Breed}{account.ReplacementConfiguration.SubstituteCharacterCreation.Sex}_{account.ReplacementConfiguration.SubstituteCharacterCreation.Head + 1}.png"));
                }
                
            }
        }
        private void TabReplacement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ManualReplacementTab.IsSelected)
            {
                LbAccountsListReplacement.ItemsSource = GlobalConfiguration.Instance.AccountsReplacementList;
                LbAccountsListReplacement.Items.Refresh();
            }
            else if (AutoReplacementTab.IsSelected)
            {
                LbAccountsListReplacement.ItemsSource = GlobalConfiguration.Instance.Accounts;
                LbAccountsListReplacement.Items.Refresh();
            }
        }

        public void UpdateAvailableSubstituteAccounts()
        {
            LabelAvailableAccounts.Text = "";
            int availableAccounts = GlobalConfiguration.Instance.SubstituteAccounts == null ? 0 : GlobalConfiguration.Instance.SubstituteAccounts.Count;
            LabelAvailableAccounts.Inlines.Add(new Bold(new Run($"({availableAccounts}) ")));
            LabelAvailableAccounts.Inlines.Add(new Run(LanguageManager.Translate("763")));
        }
        private void BtnCharacterCreator_Click(object sender, RoutedEventArgs e)
        {
            if (LbAccountsListReplacement.SelectedItems == null || LbAccountsListReplacement.SelectedItems?.Count <= 0)
                return;

            var selectedAccounts = LbAccountsListReplacement.SelectedItems.Cast<AccountConfiguration>().ToList();

            var accountCreatorInterface = new AccountsCharacterCreator(selectedAccounts, this, true);
            accountCreatorInterface.ShowDialog();
        }
        private void BtnApplyReplacementSettings_Click(object sender, RoutedEventArgs e)
        {
            if (LbAccountsListReplacement.SelectedItems == null || LbAccountsListReplacement.SelectedItems?.Count <= 1)
                return;

            var accounts = LbAccountsListReplacement.SelectedItems.Cast<AccountConfiguration>().ToList();
            foreach (var account in accounts)
            {
                account.ReplacementConfiguration.ActivateReplacement = TempActivateReplacement;
                account.ReplacementConfiguration.ConnectAfterReplacement = TempConnectAfterReplacement;
                account.ReplacementConfiguration.StartScript = TempStartScript;
                account.ReplacementConfiguration.SubstituteCharacterCreation.Create = TempCreateCharacter;
                account.ReplacementConfiguration.DisconnectTimer = TempDisconnectTimer;
            }

            UpdateAccountsUI();

        }

        #endregion

        private void UpdateAccountsUI()
        {
            LvAccounts.ItemsSource = GlobalConfiguration.Instance.AccountsList;
            LvAccounts.Items.Refresh();
            LbAccounts.ItemsSource = GlobalConfiguration.Instance.AccountsList;
            LbAccounts.Items.Refresh();
        }

        private void LbAccountsListReplacement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LbAccountsListReplacement_SelectionChangedCustom(sender, e);
        }
    }
}