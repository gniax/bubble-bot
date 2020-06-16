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


        // Constructor
        public AccountConfiguration(string username, string password, string server, string character, string nickname,
            string identifiant, bool isban)
        {
            Username = username;
            Password = password;
            Server = server;
            Character = character;
            Nickname = nickname;
            Identifiant = identifiant;
            IsBan = isban;
            Proxy = new ProxyConfiguration();
            CharacterCreation = new CharacterCreation();
            Planification = new ObservableCollection<bool>(Enumerable.Repeat(false, 24));
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

        [JsonProperty("IsBan")]
        public bool IsBan { get; set; }

        [JsonProperty("Proxy")]
        public ProxyConfiguration Proxy { get; private set; }

        [JsonProperty("CharacterCreation")]
        public CharacterCreation CharacterCreation { get; set; }

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
        public SolidColorBrush UsernameColor =>
            IsBan ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);

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
}