using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Groups;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BubbleBot.Configurations
{
    public class GlobalConfiguration : ViewModelBase
    {
        // Fields
        public readonly string _configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.bbot");
        public bool IsProxyValid => _proxyIp.Length > 0;
        public string ProxyUrl => _proxyIp.Length > 0 ? $"http://{_proxyIp}:{_proxyPort}" : "";

        private readonly SemaphoreSlim _semaphore;
        private string _antiCaptchaKey;
        private bool _automaticReconnection;
        private bool _displayItemsInLogs;
        private Languages _language;
        private bool _loaded;
        private bool _randomNickname;
        private bool _showDebugMessages;
        private string _proxyIp;
        private ushort _proxyPort;
        private string _proxyUsername;
        private string _proxyPassword;


        // Constructor
        private GlobalConfiguration()
        {
            Accounts = new ObservableCollection<AccountConfiguration>();

            AntiCaptchaKey = "";
            ShowDebugMessages = true;
            DisplayItemsInLogs = true;
            RandomNickname = false;
            AutomaticReconnection = true;
            Username = "";
            ProxyIp = "";
            ProxyPort = 0;
            ProxyUsername = "";
            ProxyPassword = "";
            Language = Languages.FRENCH;
            _semaphore = new SemaphoreSlim(1, 1);
        }


        // Properties
        public ObservableCollection<AccountConfiguration> Accounts { get; set; }

        public string AntiCaptchaKey
        {
            get => _antiCaptchaKey;
            set
            {
                Set(ref _antiCaptchaKey, value);
                Save();
            }
        }

        public bool ShowDebugMessages
        {
            get => _showDebugMessages;
            set
            {
                Set(ref _showDebugMessages, value);
                Save();
            }
        }

        public bool DisplayItemsInLogs
        {
            get => _displayItemsInLogs;
            set
            {
                Set(ref _displayItemsInLogs, value);
                Save();
            }
        }

        public bool RandomNickname
        {
            get => _randomNickname;
            set
            {
                Set(ref _randomNickname, value);
                Save();
            }
        }

        public bool AutomaticReconnection
        {
            get => _automaticReconnection;
            set
            {
                Set(ref _automaticReconnection, value);
                Save();
            }
        }
        public string ProxyIp
        {
            get => _proxyIp;
            set
            {
                Set(ref _proxyIp, value);
                Save();
            }
        }
        public ushort ProxyPort
        {
            get => _proxyPort;
            set
            {
                Set(ref _proxyPort, value);
                Save();
            }
        }

        public string ProxyUsername
        {
            get => _proxyUsername;
            set
            {
                Set(ref _proxyUsername, value);
                Save();
            }
        }

        public string ProxyPassword
        {
            get => _proxyPassword;
            set
            {
                Set(ref _proxyPassword, value);
                Save();
            }
        }
        public string Username { get; set; }

        public Languages Language
        {
            get => _language;
            set
            {
                Set(ref _language, value);
                Save();
            }
        }

        public List<AccountConfiguration> AccountsList
        {
            get
            {
                var list = new List<AccountConfiguration>(Accounts);
                foreach (var connectedEntity in BubbleBotMain.Instance.Entities)
                    if (connectedEntity is Account a)
                    {
                        list.Remove(a.AccountConfig);
                    }
                    else if (connectedEntity is Group group)
                    {
                        list.Remove(@group.Chief.AccountConfig);
                        foreach (var member in @group.Members) list.Remove(member.AccountConfig);
                    }

                return list;
            }
        }

        public string Lang => Language == Languages.ENGLISH ? "en" : Language == Languages.FRENCH ? "fr" : "pt";


        public void Load()
        {
            _loaded = false;

            if (File.Exists(_configPath))
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Accounts.Clear();
                    try
                    {
                        using (var sr = new StreamReader(File.Open(_configPath, FileMode.Open), Encoding.UTF8))
                        {
                            var json = JObject.Parse(sr.ReadToEnd());

                            AntiCaptchaKey = json.SelectToken("AntiCaptchaKey").Value<string>();
                            ShowDebugMessages = json.SelectToken("ShowDebugMessages").Value<bool>();
                            DisplayItemsInLogs = json.SelectToken("DisplayItemsInLogs").Value<bool>();
                            RandomNickname = json.SelectToken("RandomNickname").Value<bool>();
                            AutomaticReconnection = json.SelectToken("AutomaticReconnection").Value<bool>();
                            Username = json.SelectToken("Username").Value<string>();
                            Language = (Languages) json.SelectToken("Language").Value<byte>();
                            ProxyIp = json.SelectToken("Proxy.Ip").Value<string>();
                            ProxyPort = json.SelectToken("Proxy.Port").Value<ushort>();
                            ProxyUsername = json.SelectToken("Proxy.Username").Value<string>();
                            ProxyPassword = json.SelectToken("Proxy.Password").Value<string>();

                            var value = json["Accounts"];
                            var accounts = value.ToObject<List<AccountConfiguration>>();

                            foreach (var acc in accounts)
                            {
                                if (!Accounts.Contains(acc))
                                {
                                    while (acc.Planification.Count > 24)
                                    {
                                        acc.Planification.RemoveAt(0);
                                    }
                                    Accounts.Add(acc);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignored
                    }
                });

            _loaded = true;
        }

        public void Save()
        {
            // Avoid saving while we're just loading
            if (!_loaded)
                return;

            _semaphore.Wait();

            using (var sw = new StreamWriter(File.Open(_configPath, FileMode.Create), Encoding.UTF8))
            {
                dynamic json = new ExpandoObject();
                json.Settings = new ExpandoObject();
                json.AntiCaptchaKey = AntiCaptchaKey;
                json.ShowDebugMessages = ShowDebugMessages;
                json.DisplayItemsInLogs = DisplayItemsInLogs;
                json.RandomNickname = RandomNickname;
                json.AutomaticReconnection = AutomaticReconnection;
                json.Username = Username;
                json.Language = (byte) Language;
                json.Proxy = new ExpandoObject();
                json.Proxy.Ip = ProxyIp;
                json.Proxy.Port = ProxyPort;
                json.Proxy.Username = ProxyUsername;
                json.Proxy.Password = ProxyPassword;
                json.Accounts = Accounts;              

                var serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(sw, json);
            }

            _semaphore.Release();
        }

        public void AddAccountAndSave(string username, string password, string server, string character,
            string nickname, string identifiant, bool isban)
        {
            Accounts.Add(new AccountConfiguration(username, password, server, character, nickname, identifiant, isban));
            RaisePropertyChanged("AccountsList");
            Save();
        }

        public void AddAccountsAndSave(IEnumerable<AccountConfiguration> accounts)
        {
            foreach (var account in accounts) Accounts.Add(account);

            RaisePropertyChanged("AccountsList");
            Save();
        }

        public void RemoveAccount(AccountConfiguration accountConfig)
        {
            Accounts.Remove(accountConfig);
            RaisePropertyChanged("AccountsList");
        }

        public void SortAccountPseudo(List<AccountConfiguration> accounttosort)
        {
            var tempAccountChecker = Accounts;
            var newAccountsSorter = new ObservableCollection<AccountConfiguration>();
            var allPseudo = tempAccountChecker.Select(a => a.Nickname).ToList();
            var allPseudoClean = new List<string>();

            //Delete les doublons 
            foreach (var Pseudo in allPseudo)
            {
                var check = false;
                foreach (var PseudoD in allPseudoClean)
                    if (Pseudo == PseudoD)
                        check = true;
                if (check == false) allPseudoClean.Add(Pseudo);
            }

            //Trier les compte par groupe 
            foreach (var actualGroup in allPseudoClean)
            foreach (var selectedAccount in tempAccountChecker)
                if (actualGroup == selectedAccount.Nickname)
                    newAccountsSorter.Add(selectedAccount);
            Accounts = newAccountsSorter;

            RaisePropertyChanged("AccountsList");
        }

        public void SetAccountPseudo(AccountConfiguration accountConfig, string selectedPseudo)
        {
            //On modifie le pseudo
            var newNickname = selectedPseudo;
            accountConfig.Nickname = newNickname;

            RaisePropertyChanged("AccountsList");
        }

        public AccountConfiguration GetAccount(string username)
        {
            return Accounts.FirstOrDefault(a => a.Username == username);
        }

        #region Singleton

        private static GlobalConfiguration _instance;

        public static GlobalConfiguration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GlobalConfiguration();

                return _instance;
            }
        }

        #endregion
    }
}