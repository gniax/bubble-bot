using BubbleBot.Server.Messages;
using BubbleBot.Server.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ExtensionsEnum = BubbleBot.Protocol.Server.Enums.Extensions;

namespace BubbleBot.Server.Clients
{
    public class ClientInformations : IDisposable
    {

        // Fields
        private Client _client;
        private Timer _refreshTimer;


        // Properties
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Password { get; private set; }
        public DateTime? TouchEndDate { get; private set; }
        public Dictionary<ExtensionsEnum, DateTime> Extensions { get; private set; }

        public bool IsSubscribedToTouch => TouchEndDate != null && DateTime.Now < TouchEndDate;
        public int MaxAccounts => !IsSubscribedToTouch ? 1 : 120;
        public int MaxInstances => !IsSubscribedToTouch ? 1 : HasExtension(ExtensionsEnum.ThirdInstance) ? 3 : 2;


        // Constructor
        public ClientInformations(Client client)
        {
            _client = client;
            _refreshTimer = new Timer(Refresh_Callback, null, Timeout.Infinite, Timeout.Infinite);

            SetDefault();
        }


        public void Set(JObject json, string password)
        {
            Id = json["user"].Value<int>("id");
            Name = json["user"].Value<string>("name");
            Password = password;

            if (json.Value<bool>("subscribed"))
            {
                TouchEndDate = json.Value<DateTime?>("touchEndDate");

                if (json["extensions"] != null)
                {
                    Extensions = json["extensions"].ToObject<Dictionary<string, DateTime>>().ToDictionary(k => (ExtensionsEnum)int.Parse(k.Key), v => v.Value);
                }
            }
        }

        public void SetDefault()
        {
            Id = -1;
            Name = "";
            Password = "";
            TouchEndDate = null;
            Extensions = new Dictionary<ExtensionsEnum, DateTime>();
        }

        public void StartRefreshing()
            => _refreshTimer.Change(300000, 300000);

        public bool HasExtension(ExtensionsEnum extension)
            => Extensions?.ContainsKey(extension) == true && Extensions?[extension] > DateTime.Now;

        public override string ToString()
            => $"({Id}:{Name})";

        private async void Refresh_Callback(object state)
        {
            Console.WriteLine("Refreshing the informations of the client {0}..", ToString());

            var response = await HttpClientUtility.GetJsonAsync($"login?username={Name}&password={Password}&token=1997");

            // In case something goes wrong
            if (response == null)
                return;

            // Login succeeded
            if (response.Value<bool>("success"))
            {
                Set(response, Password);
            }
            // Login failed (which doesn't have to happen)
            else
            {
                SetDefault();
                _client.Network.Close();
            }

            _client.SendMessage(new SubscriptionInformationsMessage(TouchEndDate, Extensions));
        }

        public void Dispose()
        {
            Name = null;
            Password = null;
            TouchEndDate = null;
            Extensions.Clear();
            Extensions = null;
            _refreshTimer.Dispose();
            _refreshTimer = null;
            _client = null;
        }

    }
}
