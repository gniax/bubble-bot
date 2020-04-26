using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using BubbleBot.Server.Clients.Accounts;
using CefSharp.OffScreen;
using CefSharp;
using System.Runtime.CompilerServices;

namespace BubbleBot.Server.Utility
{
    public static class SetVersions
    {
        private static ChromiumWebBrowser _browser;
        // Note: this function retrieves the build, app, assets and static data versions
        // this uses Lindo emu to get build and app version, and directly dofus server the both remaining
        // it could be optimize with some HttpRequest / return true : works / return false : didnt works
        private static string _staticDataVersion = null;
        private static bool _endFrame = false;
        public static bool setVersions()
        {
            //LoadBrowser();
            try
            {
                string JSONversions = PerformXmlHttpRequest("http://api.no-emu.co/version.json");
                Dictionary<string, object> dictionaryVersions = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(JSONversions));

                if ((string)dictionaryVersions["buildVersion"] != null && (string)dictionaryVersions["buildVersion"] != "null")
                Constants.BuildVersion = (string)dictionaryVersions["buildVersion"];
                if ((string)dictionaryVersions["appVersion"] != null && (string)dictionaryVersions["appVersion"] != "null")
                Constants.AppVersion = (string)dictionaryVersions["appVersion"];

                //var mainFrame = _browser.GetMainFrame();
                //var tokenRequest = mainFrame.CreateRequest(false);
                //tokenRequest.Url = "https://proxyconnection.touch.dofus.com/assetsVersions.json?staticDataVersion=0&assetsVersion=0";
                //tokenRequest.Method = "GET";
                //mainFrame.LoadRequest(tokenRequest);

                //_browser.FrameLoadEnd += delegate (object sender, FrameLoadEndEventArgs e)
                //{
                //   GetContent(RuntimeHelpers.GetObjectValue(sender), e);
                //};

                //var getToken = SpinWait.SpinUntil(() => _endFrame != false, TimeSpan.FromSeconds(20));
                //_endFrame = false;

                //Dictionary<string, object> dictionaryAssets = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(_staticDataVersion));
                //Constants.StaticDataVersion = (string)dictionaryAssets["staticDataVersion"];

                string assets = PerformXmlHttpRequest("https://panel.snowbot.eu/api/assets-version.txt");
                Constants.AssetsVersion = assets;

                //Console.WriteLine("AppV: {0} BuildV: {1}, AssetsV: {2}, StaticDataV: {3}", Constants.AppVersion, Constants.BuildVersion, Constants.AssetsVersion, Constants.StaticDataVersion);

                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine("Erreur lors de la récupération des versions: {0}", error);
                return false;
            }
        }
        public static string PerformXmlHttpRequest(string urlString)
        {
            HttpWebResponse httpWebResponse = null; //Declare an HTTP-specific implementation of the WebResponse class
            //Creates an HttpWebRequest for the specified URL.
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);

            //Set HttpWebRequest properties
            httpWebRequest.Headers.Add("Origin", "file://");
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            httpWebRequest.Headers.Add("Accept-Language", "fr");
            httpWebRequest.UserAgent =
                "Mozilla/5.0 (Linux; Android 7.0; Nexus 5X Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.118 Mobile Safari/537.36";
            httpWebRequest.ContentType = "application/json'";
            httpWebRequest.Accept = "*/*";

            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
            
        }
        private static void LoadBrowser()
        {
            if (_browser == null || _browser.IsDisposed)
            {
                var browserSettings = new BrowserSettings
                {
                    ApplicationCache = CefState.Disabled,
                    FileAccessFromFileUrls = CefState.Disabled,
                    UniversalAccessFromFileUrls = CefState.Disabled,
                    ImageLoading = CefState.Disabled,
                    Javascript = CefState.Disabled,
                    WebSecurity = CefState.Disabled,
                    Plugins = CefState.Disabled,
                    LocalStorage = CefState.Disabled,
                    WebGl = CefState.Disabled,
                    WindowlessFrameRate = 1
                };

                _browser = new ChromiumWebBrowser("about:blank", browserSettings, new RequestContext());


                var browserInit = SpinWait.SpinUntil(() => _browser.IsBrowserInitialized, TimeSpan.FromSeconds(20));
            }
        }

        public static void GetContent(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                if (e.HttpStatusCode == 200)
                {
                    e.Frame.GetTextAsync().ContinueWith(taskHtml =>
                    {
                        var resultHtml = taskHtml.Result;
                        _staticDataVersion = resultHtml;
                        _endFrame = true;
                    });
                }
            }
        }
    }
}
