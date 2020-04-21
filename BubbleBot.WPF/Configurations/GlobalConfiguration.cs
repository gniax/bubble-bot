using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Groups;
using GalaSoft.MvvmLight;

namespace BubbleBot.Configurations
{
    public class GlobalConfiguration : ViewModelBase
    {
        // Fields
        public readonly string _configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.bbot");
        private readonly SemaphoreSlim _semaphore;
        private string _antiCaptchaKey;
        private bool _automaticReconnection;
        private bool _displayItemsInLogs;
        private Languages _language;
        private bool _loaded;
        private bool _randomNickname;
        private bool _showDebugMessages;


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
                        using (var br = new BinaryReader(File.Open(_configPath, FileMode.Open)))
                        {
                            var c = br.ReadInt32();
                            for (var i = 0; i < c; i++) Accounts.Add(AccountConfiguration.Load(br));

                            AntiCaptchaKey = br.ReadString();
                            ShowDebugMessages = br.ReadBoolean();
                            DisplayItemsInLogs = br.ReadBoolean();
                            RandomNickname = br.ReadBoolean();
                            AutomaticReconnection = br.ReadBoolean();
                            Username = br.ReadString();
                            Language = (Languages) br.ReadByte();

                            for (var i = 0; i < Accounts.Count; i++)
                            {
                                for (var j = 0; j < 24; j++)
                                    Accounts[i].Planification[j] = br.ReadBoolean();

                                Accounts[i].PlanificationActivated = br.ReadBoolean();
                                Accounts[i].ForceStartScript = br.ReadBoolean();
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

            using (var bw = new BinaryWriter(File.Open(_configPath, FileMode.Create)))
            {
                bw.Write(Accounts.Count);
                foreach (var accountConfig in Accounts) accountConfig?.Save(bw);

                bw.Write(AntiCaptchaKey);
                bw.Write(ShowDebugMessages);
                bw.Write(DisplayItemsInLogs);
                bw.Write(RandomNickname);
                bw.Write(AutomaticReconnection);
                bw.Write(Username);
                bw.Write((byte) Language);

                for (var i = 0; i < Accounts.Count; i++)
                {
                    for (var j = 0; j < 24; j++)
                        bw.Write(Accounts[i].Planification[j]);

                    bw.Write(Accounts[i].PlanificationActivated);
                    bw.Write(Accounts[i].ForceStartScript);
                }
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
            var allPseudo = new List<string>();
            var allPseudoClean = new List<string>();

            //Creer une liste des pseudo avant ? 
            foreach (var ActualPseudo in tempAccountChecker) allPseudo.Add(ActualPseudo.Nickname);

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