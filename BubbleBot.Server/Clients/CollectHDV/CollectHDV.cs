using BubbleBot.Server.Utility;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BubbleBot.Server.Clients.CollectHDV
{
    public class CollectHDV
    {
        public int ObjectId { get; private set; }
        public string ObjectName { get; private set; }
        public string Server { get; private set; }
        public int PriceLot1 { get; private set; }
        public int PriceLot10 { get; private set; }
        public int PriceLot100 { get; private set; }
        public int AveragePrice { get; private set; }

        // Constructor
        public CollectHDV() {}

        public async void SendItemCollectedInformations(int objectid, string objectname, string server, int pricelot1, int pricelot10, int pricelot100, int averageprice)
        {
            ObjectId = objectid;
            ObjectName = objectname;
            Server = server;
            PriceLot1 = pricelot1;
            PriceLot10 = pricelot10;
            PriceLot100 = pricelot100;
            AveragePrice = averageprice;

            await HttpClientUtility.PostAsync($"collected/hdv", GeneratePostContent());
        }

        private FormUrlEncodedContent GeneratePostContent()
    => new FormUrlEncodedContent(new[]
       {
                   new KeyValuePair<string, string>("token", "1997"),
                   new KeyValuePair<string, string>("object_id", ObjectId.ToString()),
                   new KeyValuePair<string, string>("object_name", ObjectName),
                   new KeyValuePair<string, string>("object_server", Server),
                   new KeyValuePair<string, string>("object_price_lot_1", PriceLot1.ToString()),
                   new KeyValuePair<string, string>("object_price_lot_10", PriceLot10.ToString()),
                   new KeyValuePair<string, string>("object_price_lot_100", PriceLot100.ToString()),
                   new KeyValuePair<string, string>("object_average_price", AveragePrice.ToString())
       });

    }
}
