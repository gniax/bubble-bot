using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Windows;
using BubbleBot.Core.Enums;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BubbleBot.Core.Accounts.Configurations
{
    public class Configuration : ViewModelBase, IDisposable
    {
        // Fields
        public const string ConfigurationsPath = "Parameters";
        private bool _acceptAchivements;
        private Account _account;
        private bool _autoMount;
        private bool _createParty;
        private bool _autoRegenAccepted;
        private int _banReconnectionDelay;
        private bool _disconnectOnBan;
        private bool _disconnectUponFightsLimit;
        private bool _ignoreNonAuthorizedTrades;
        private bool _loaded;
        private bool _showAllianceMessages;
        private bool _showFightMessages;
        private bool _showGeneralMessages;
        private bool _showGuildMessages;
        private bool _showNoobMessages;
        private bool _showPartyMessages;
        private bool _showSaleMessages;
        private bool _showSeekMessages;
        private bool _speedHack;
        private BoostableStats _statToBoost;


        // Constructor
        public Configuration(Account account)
        {
            _account = account;

            ShowGeneralMessages = true;
            ShowPartyMessages = true;
            ShowFightMessages = true;
            ShowGuildMessages = true;
            ShowAllianceMessages = true;
            ShowSaleMessages = true;
            ShowSeekMessages = true;
            ShowNoobMessages = true;
            CreateParty = true;
            AutoRegenAccepted = true;
            AcceptAchievements = true;
            StatToBoost = BoostableStats.NONE;
            SpellsToBoost = new ObservableCollection<SpellToBoostEntry>();
            AuthorizedTradesFrom = new ObservableCollection<int>();
            IgnoreNonAuthorizedTrades = false;
            DisconnectUponFightsLimit = false;
            DisconnectOnBan = false;
            BanReconnectionDelay = 0;
            SpeedHack = false;
            AutoMount = true;
        }


        // Properties
        public bool ShowGeneralMessages
        {
            get => _showGeneralMessages;
            set
            {
                Set(ref _showGeneralMessages, value);
                Save();
            }
        }

        public bool ShowPartyMessages
        {
            get => _showPartyMessages;
            set
            {
                Set(ref _showPartyMessages, value);
                Save();
            }
        }

        public bool ShowFightMessages
        {
            get => _showFightMessages;
            set
            {
                Set(ref _showFightMessages, value);
                Save();
            }
        }

        public bool ShowGuildMessages
        {
            get => _showGuildMessages;
            set
            {
                Set(ref _showGuildMessages, value);
                Save();
            }
        }

        public bool ShowAllianceMessages
        {
            get => _showAllianceMessages;
            set
            {
                Set(ref _showAllianceMessages, value);
                Save();
            }
        }

        public bool ShowSaleMessages
        {
            get => _showSaleMessages;
            set
            {
                Set(ref _showSaleMessages, value);
                Save();
            }
        }

        public bool ShowSeekMessages
        {
            get => _showSeekMessages;
            set
            {
                Set(ref _showSeekMessages, value);
                Save();
            }
        }

        public bool ShowNoobMessages
        {
            get => _showNoobMessages;
            set
            {
                Set(ref _showNoobMessages, value);
                Save();
            }
        }

        public bool CreateParty
        {
            get => _createParty;
            set
            {
                Set(ref _createParty, value);
                Save();
            }
        }

        public bool AutoRegenAccepted
        {
            get => _autoRegenAccepted;
            set
            {
                Set(ref _autoRegenAccepted, value);
                Save();
            }
        }

        public bool AcceptAchievements
        {
            get => _acceptAchivements;
            set
            {
                Set(ref _acceptAchivements, value);
                Save();
            }
        }

        public BoostableStats StatToBoost
        {
            get => _statToBoost;
            set
            {
                Set(ref _statToBoost, value);
                Save();
            }
        }

        [JsonProperty("SpellsToBoost")]
        public ObservableCollection<SpellToBoostEntry> SpellsToBoost { get; private set; }
        [JsonProperty("AuthorizedTradesFrom")]
        public ObservableCollection<int> AuthorizedTradesFrom { get; private set; }

        public bool IgnoreNonAuthorizedTrades
        {
            get => _ignoreNonAuthorizedTrades;
            set
            {
                Set(ref _ignoreNonAuthorizedTrades, value);
                Save();
            }
        }

        public bool DisconnectUponFightsLimit
        {
            get => _disconnectUponFightsLimit;
            set
            {
                Set(ref _disconnectUponFightsLimit, value);
                Save();
            }
        }

        public bool SpeedHack
        {
            get => _speedHack;
            set
            {
                Set(ref _speedHack, value);
                Save();
            }
        }

        public int BanReconnectionDelay
        {
            get => _banReconnectionDelay;
            set
            {
                Set(ref _banReconnectionDelay, value);
                Save();
            }
        }

        public bool DisconnectOnBan
        {
            get => _disconnectOnBan;
            set
            {
                Set(ref _disconnectOnBan, value);
                Save();
            }
        }

        public bool AutoMount
        {
            get => _autoMount;
            set
            {
                Set(ref _autoMount, value);
                Save();
            }
        }

        private string ConfigFilePath => Path.Combine(ConfigurationsPath, $"{_account.AccountConfig.Username}.config");


        public void Load()
        {
            _loaded = false;

            if (File.Exists(ConfigFilePath))
                try
                {
                    using (var sr = new StreamReader(File.Open(ConfigFilePath, FileMode.Open, FileAccess.ReadWrite,
                        FileShare.ReadWrite), Encoding.UTF8))
                    {

                        var json = JObject.Parse(sr.ReadToEnd());
                        CreateParty = json["CreateParty"] != null ? (bool)json["CreateParty"] : true;
                        AutoRegenAccepted = json["AutoRegenAccepted"] != null ? (bool)json["AutoRegenAccepted"] : true;
                        AcceptAchievements = json["AcceptAchievements"] != null ? (bool)json["AcceptAchievements"] : true;
                        StatToBoost = json["StatToBoost"] != null ? (BoostableStats)(byte)json["StatToBoost"] : BoostableStats.NONE;
                        IgnoreNonAuthorizedTrades = json["IgnoreNonAuthorizedTrades"] != null ? (bool)json["IgnoreNonAuthorizedTrades"] : false;
                        DisconnectUponFightsLimit = json["DisconnectUponFightsLimit"] != null ? (bool)json["DisconnectUponFightsLimit"] : false;
                        SpeedHack = json["SpeedHack"] != null ? (bool)json["SpeedHack"] : false;
                        DisconnectOnBan = json["DisconnectOnBan"] != null ? (bool)json["DisconnectOnBan"] : false;
                        BanReconnectionDelay = json["BanReconnectionDelay"] != null ? (int)json["BanReconnectionDelay"] : 0;
                        AutoMount = json["AutoMount"] != null ? (bool)json["AutoMount"] : true;
                        ShowGeneralMessages = json["Channel"]["ShowGeneralMessages"] != null ? (bool)json["Channel"]["ShowGeneralMessages"] : true;
                        ShowPartyMessages = json["Channel"]["ShowPartyMessages"] != null ? (bool)json["Channel"]["ShowPartyMessages"] : true;
                        ShowFightMessages = json["Channel"]["ShowFightMessages"] != null ? (bool)json["Channel"]["ShowFightMessages"] : true;
                        ShowGuildMessages = json["Channel"]["ShowGuildMessages"] != null ? (bool)json["Channel"]["ShowGuildMessages"] : true;
                        ShowAllianceMessages = json["Channel"]["ShowAllianceMessages"] != null ? (bool)json["Channel"]["ShowAllianceMessages"] : true;
                        ShowSaleMessages = json["Channel"]["ShowSaleMessages"] != null ? (bool)json["Channel"]["ShowSaleMessages"] : true;
                        ShowSeekMessages = json["Channel"]["ShowSeekMessages"] != null ? (bool)json["Channel"]["ShowSeekMessages"] : true;
                        ShowNoobMessages = json["Channel"]["ShowNoobMessages"] != null ? (bool)json["Channel"]["ShowNoobMessages"] : true;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SpellsToBoost.Clear();
                            var value = json["SpellsToBoost"];
                            if (value != null)
                            {
                                var spells = value.ToObject<List<SpellToBoostEntry>>();
                                foreach (var spell in spells)
                                {
                                    if (!SpellsToBoost.Contains(spell))
                                    {
                                        SpellsToBoost.Add(new SpellToBoostEntry(spell.Id, spell.Name, spell.Level));
                                    }
                                    else
                                    {
                                        SpellsToBoost[SpellsToBoost.IndexOf(spell)] = spell;
                                    }
                                }
                            }

                            AuthorizedTradesFrom.Clear();
                            value = json["AuthorizedTradesFrom"];
                            if (value != null)
                            {
                                var ids = value.ToObject<List<int>>();
                                foreach (var id in ids)
                                {
                                    if (!AuthorizedTradesFrom.Contains(id))
                                    {
                                        AuthorizedTradesFrom.Add(id);
                                    }
                                }
                            }
                        });

                    }
                }
                catch
                {
                }

            _loaded = true;
        }

        public void Save()
        {
            // Avoid saving while we're just loading
            if (!_loaded)
                return;

            // Ensure that the directory is created
            Directory.CreateDirectory(ConfigurationsPath);

            try
            {
                using (var sw = new StreamWriter(File.Open(ConfigFilePath, FileMode.Create, FileAccess.ReadWrite,
                    FileShare.ReadWrite), Encoding.UTF8))
                {
                    dynamic json = new ExpandoObject();
                    json.CreateParty = CreateParty;
                    json.AutoRegenAccepted = AutoRegenAccepted;
                    json.AcceptAchievements = AcceptAchievements;
                    json.StatToBoost = (byte) StatToBoost;
                    json.IgnoreNonAuthorizedTrades = IgnoreNonAuthorizedTrades;
                    json.DisconnectUponFightsLimit = DisconnectUponFightsLimit;
                    json.SpeedHack = SpeedHack;
                    json.DisconnectOnBan = DisconnectOnBan;
                    json.BanReconnectionDelay = BanReconnectionDelay;
                    json.AutoMount = AutoMount;

                    json.Channel = new ExpandoObject();
                    json.Channel.ShowGeneralMessages = ShowGeneralMessages;
                    json.Channel.ShowPartyMessages = ShowPartyMessages;
                    json.Channel.ShowFightMessages = ShowFightMessages;
                    json.Channel.ShowGuildMessages = ShowGuildMessages;
                    json.Channel.ShowAllianceMessages = ShowAllianceMessages;
                    json.Channel.ShowSaleMessages = ShowSaleMessages;
                    json.Channel.ShowSeekMessages = ShowSeekMessages;
                    json.Channel.ShowNoobMessages = ShowNoobMessages;
                    json.SpellsToBoost = SpellsToBoost;
                    json.AuthorizedTradesFrom = AuthorizedTradesFrom;

                    var serializer = new JsonSerializer();
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(sw, json);
                }
            }
            catch
            {
            }
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _account = null;
                SpellsToBoost = null;
                AuthorizedTradesFrom = null;

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}