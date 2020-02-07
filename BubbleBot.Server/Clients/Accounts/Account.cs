using BubbleBot.Server.Utility;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace BubbleBot.Server.Clients.Accounts
{
    public class Account
    {

        // Properties
        public string Username { get; }
        public int BotId { get; private set; }
        public string BotName { get; private set; }
        public string BotServer { get; private set; }
        public byte BotLevel { get; private set; }
        public byte BotEnergyPercent { get; private set; }
        public byte BotWeightPercent { get; private set; }
        public int BotKamas { get; private set; }
        public int BotMapId { get; private set; }
        public string BotMapPosition { get; private set; }
        public string BotState { get; private set; }

        public bool HasBot => BotId > 0;


        // Constructor
        public Account(string username)
        {
            Username = username;
        }


        public async void SetInitialBotInformations(int clientId, int botId, string botName, string botServer, byte botLevel)
        {
            BotId = botId;
            BotName = botName;
            BotServer = botServer;
            BotLevel = botLevel;
            BotMapPosition = "-";
            BotState = "-";

            await HttpClientUtility.PostAsync("characters", GeneratePostContent(clientId));
        }

        public async void UpdateBotInformations(int clientId, byte botLevel, byte botEnergyPercent, byte botWeightPercent, int botKamas, int botMapId, string botMapPosition, string botState)
        {
            BotLevel = botLevel;
            BotEnergyPercent = botEnergyPercent;
            BotWeightPercent = botWeightPercent;
            BotKamas = botKamas;
            BotMapId = botMapId;
            BotMapPosition = botMapPosition;
            BotState = botState;

            await HttpClientUtility.PatchAsync($"characters/{BotId}", GeneratePostContent(clientId));
        }

        private FormUrlEncodedContent GeneratePostContent(int clientId)
            => new FormUrlEncodedContent(new[]
               {
                   new KeyValuePair<string, string>("token", "1997"),
                   new KeyValuePair<string, string>("user_id", clientId.ToString()),
                   new KeyValuePair<string, string>("character_id", BotId.ToString()),
                   new KeyValuePair<string, string>("account", Username),
                   new KeyValuePair<string, string>("name", BotName),
                   new KeyValuePair<string, string>("server", BotServer),
                   new KeyValuePair<string, string>("level", BotLevel.ToString()),
                   new KeyValuePair<string, string>("percent_energy", BotEnergyPercent.ToString()),
                   new KeyValuePair<string, string>("percent_pods", BotWeightPercent.ToString()),
                   new KeyValuePair<string, string>("kamas", BotKamas.ToString()),
                   new KeyValuePair<string, string>("map_id", BotMapId.ToString()),
                   new KeyValuePair<string, string>("map_pos", BotMapPosition),
                   new KeyValuePair<string, string>("state", BotState)
               });

    }
}
