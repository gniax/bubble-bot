using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Server.Utility
{
    class SetVersions
    {

        // Note: this function retrieves the build, app, assets and static data versions
        // this uses Lindo emu to get build and app version, and directly dofus server the both remaining
        // it could be optimize with some HttpRequest / return true : works / return false : didnt works
        public static bool setVersions()
        {
            try
            {
                WebClient webClient = new WebClient();
                string JSONversions = webClient.DownloadString("http://api.no-emu.co/version.json");
                Dictionary<string, object> dictionaryVersions = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(JSONversions));
                Constants.BuildVersion = (string)dictionaryVersions["buildVersion"];
                Constants.AppVersion = (string)dictionaryVersions["appVersion"];

                string JSONassets = webClient.DownloadString("https://proxyconnection.touch.dofus.com/assetsVersions.json?staticDataVersion=0&assetsVersion=0");
                Dictionary<string, object> dictionaryAssets = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(JSONassets));
                Constants.StaticDataVersion = (string)dictionaryAssets["staticDataVersion"];

                string assets = webClient.DownloadString("https://panel.snowbot.eu/api/assets-version.txt");
                Constants.AssetsVersion = assets;

                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine("Erreur lors de la récupération des versions: {0}", error);
                return false;
            }
        }
    }
}
