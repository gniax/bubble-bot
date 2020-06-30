using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using BubbleBot.Utility.Security;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace BubbleBot.Configurations
{
    public class AccountConfiguration : ViewModelBase
    {
        private bool _forceStartScript;
        // Fields
        private bool _planificationActivated;

        // --------------------------------------------------------------------------- //
        // ----------------------------- IMPORTANT NOTES ----------------------------- //
        // ------------------------ STATE : 0 => VALID ACCOUNT ----------------------- //
        // --------------- STATE : 1 => INVALID ACCOUNT : EMAIL ADDRESS -------------- //
        // ------------ STATE : 2 => INVALID ACCOUNT : WRONG CREDENTIALS ------------- //
        // ----------------------- STATE : 3 => ACCOUNT IS BAN ----------------------- //
        // --------------------------------------------------------------------------- //

        // Constructor
        public AccountConfiguration(string username, string password, string server, string character, string nickname,
            string identifiant, short state)
        {
            Username = username;
            Password = password;
            Server = server;
            Character = character;
            Nickname = nickname;
            Identifiant = identifiant;
            State = state;
            Proxy = new ProxyConfiguration();
            CharacterCreation = new CharacterCreation();
            Planification = new ObservableCollection<bool>(Enumerable.Repeat(false, 24));
            ReplacementConfiguration = new ReplacementConfiguration();
        }


        // Properties
        public string Username { get; set; }
        [JsonConverter(typeof(EncryptingJsonConverter), "Bûbbl€Bôt")]
        public string Password { get; set; }

        [JsonProperty("Server")]
        public string Server { get; set; }

        [JsonProperty("Character")]
        public string Character { get; set; }

        [JsonProperty("Nickname")]
        public string Nickname { get; set; }

        [JsonProperty("Identifiant")]
        public string Identifiant { get; set; }

        [JsonProperty("State")]
        public short State { get; set; }

        [JsonProperty("IsBan")]
        public bool IsBan => State == 3;

        [JsonProperty("Proxy")]
        public ProxyConfiguration Proxy { get; private set; }

        [JsonProperty("CharacterCreation")]
        public CharacterCreation CharacterCreation { get; set; }

        [JsonProperty("ReplacementConfiguration")]
        public ReplacementConfiguration ReplacementConfiguration { get; set; }

        [JsonProperty("PlanificationActivated")]
        public bool PlanificationActivated
        {
            get => _planificationActivated;
            set
            {
                Set(ref _planificationActivated, value);
                GlobalConfiguration.Instance.Save();
            }
        }

        [JsonProperty("ForceStartScript")]
        public bool ForceStartScript
        {
            get => _forceStartScript;
            set
            {
                Set(ref _forceStartScript, value);
                GlobalConfiguration.Instance.Save();
            }
        }

        [JsonProperty("UsernameColor")]
        public SolidColorBrush UsernameColor => State == 0 ? new SolidColorBrush(Colors.White) : State == 3 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Pink);

        [JsonProperty("Planification")]
        public ObservableCollection<bool> Planification { get; }


        public void SetProxy(string ip, ushort port, string username, string password)
        {
            Proxy.Ip = ip;
            Proxy.Port = port;
            Proxy.Username = username;
            Proxy.Password = password;

            RaisePropertyChanged("Proxy");
        }
    }

    public class ProxyConfiguration
    {
        // Constructor
        public ProxyConfiguration()
        {
            Ip = "";
            Port = 0;
            Username = "";
            Password = "";
        }

        // Properties
        [JsonProperty("Ip")]
        public string Ip { get; set; }
        [JsonProperty("Port")]
        public ushort Port { get; set; }
        [JsonProperty("Username")]
        public string Username { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("IsValid")]
        public bool IsValid => Ip.Length > 0;
        [JsonProperty("Url")]
        public string Url => Ip.Length > 0 ? $"http://{Ip}:{Port}" : "";
    }

    public class ReplacementConfiguration
    {
        // Constructor
        public ReplacementConfiguration()
        {
            ActivateReplacement = false;
            ConnectAfterReplacement = false;
            StartScript = false;
            DisconnectTimer = 0;
            SubstituteCharacterCreation = new CharacterCreation();
        }

        // Properties
        [JsonProperty("ActivateReplacement")]
        public bool ActivateReplacement { get; set; }

        [JsonProperty("ConnectAfterReplacement")]
        public bool ConnectAfterReplacement { get; set; }

        [JsonProperty("StartScript")]
        public bool StartScript { get; set; }

        [JsonProperty("DisconnectTimer")]
        public int DisconnectTimer { get; set; }

        [JsonProperty("SubstituteCharacterCreation")]
        public CharacterCreation SubstituteCharacterCreation { get; set; }
    }
}