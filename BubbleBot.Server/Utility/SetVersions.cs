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
using BubbleBot.Server.Utility.ChromiumExtensions;
using System.Text.RegularExpressions;

namespace BubbleBot.Server.Utility
{
    public static class SetVersions
    {
        private static ChromiumWebBrowser _browser;
        // Note: this function retrieves the build, app, assets and static data versions
        private static string _frameContent = null;
        private static bool _endFrame = false;
        public static bool setVersions()
        {
            string host = "",
                   service = "0",
                   username = "",
                   password = "";

            if (File.Exists("proxy.txt") && new FileInfo("proxy.txt").Length != 0)
            {
                string text = File.ReadAllText("proxy.txt");
                string[] array = text.Split(':');

                host = (array[0] == null || array[0] == "") ? "" : array[0];
                service = (array[1] == null || array[1] == "") ? "0" : array[1];
                username = (array[2] == null || array[2] == "") ? "" : array[2];
                password = (array[3] == null || array[3] == "") ? "" : array[3];
            }

            LoadBrowser(host, service, username, password);

            try
            {
                string jsVersions = PerformXmlHttpRequest("https://proxyconnection.touch.dofus.com/build/script.js", host, service, username, password);
                jsVersions = jsVersions.Substring(jsVersions.IndexOf("window.buildVersion=\"") + 21);
                string buildVersion = jsVersions.Substring(0, jsVersions.IndexOf("\""));
                if (buildVersion != null && buildVersion != "null")
                Constants.BuildVersion = buildVersion;

                string appVersion = getPlayStoreAppVersion($"https://play.google.com/store/apps/details?id=com.ankama.dofustouch&hl=en");
                if (appVersion != null && appVersion != "Unknown")
                Constants.AppVersion = appVersion;

                var mainFrame = _browser.GetMainFrame();
                var staticReq = mainFrame.CreateRequest(false);
                staticReq.Url = "https://proxyconnection.touch.dofus.com/assetsVersions.json?staticDataVersion=0&assetsVersion=0";
                staticReq.Method = "GET";
                mainFrame.LoadRequest(staticReq);

                _browser.FrameLoadEnd += delegate (object sender, FrameLoadEndEventArgs e)
                {
                    GetContent(RuntimeHelpers.GetObjectValue(sender), e);
                };

                var getStaticDataVersion = SpinWait.SpinUntil(() => _endFrame != false, TimeSpan.FromSeconds(20));
                _endFrame = false;

                Dictionary<string, object> dictionaryAssets = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(_frameContent));
                Constants.StaticDataVersion = (string)dictionaryAssets["staticDataVersion"];

                mainFrame = _browser.GetMainFrame();
                var assetsReq = mainFrame.CreateRequest(false);
                assetsReq.Url = "https://proxyconnection.touch.dofus.com/config.json?";
                assetsReq.Method = "GET";
                mainFrame.LoadRequest(assetsReq);

                _browser.FrameLoadEnd += delegate (object sender, FrameLoadEndEventArgs e)
                {
                    GetContent(RuntimeHelpers.GetObjectValue(sender), e);
                };

                var getAssets = SpinWait.SpinUntil(() => _endFrame != false, TimeSpan.FromSeconds(20));
                _endFrame = false;

                Dictionary<string, object> dictionnaryConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(_frameContent));
                var assetsUrl = (string)dictionnaryConfig["assetsUrl"];
                assetsUrl = assetsUrl.Substring(assetsUrl.IndexOf("/assets/")).Substring(8);
                Constants.AssetsVersion = assetsUrl;
                //Console.WriteLine("AppV: {0} BuildV: {1}, AssetsV: {2}, StaticDataV: {3}", Constants.AppVersion, Constants.BuildVersion, Constants.AssetsVersion, Constants.StaticDataVersion);

                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine("Erreur lors de la récupération des versions: {0}", error);
                return false;
            }
        }

        private static string getPlayStoreAppVersion(string appUrlString, string host = "", string service = "0", string username = "", string password = "")
        {
            // Get the current version pattern sequence 
            string responseHTML = PerformXmlHttpRequest(appUrlString, host, service, username, password);
            if (responseHTML == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var rx = new Regex(@"(?<=""htlgb"">)(\d{1,3}\.\d{1,3}\.{0,1}\d{0,3})(?=<\/span>)", RegexOptions.Compiled);
                    MatchCollection matches = rx.Matches(responseHTML);
                    return matches.Count > 0 ? matches[0].Value : "Unknown";
                }
                catch
                {
                    return "Unknown";
                }
            }
        }

        public static string PerformXmlHttpRequest(string urlString, string host = "", string service = "0", string username = "", string password = "")
        {
            //Creates an HttpWebRequest for the specified URL.
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);

            if (host != "" && service != "0")
            {
                WebProxy proxy = new WebProxy(host, Int32.Parse(service));
                proxy.BypassProxyOnLocal = false;
                httpWebRequest.Proxy = proxy;
                httpWebRequest.Method = "GET";
                proxy = (WebProxy)httpWebRequest.Proxy;

                if (username != "" && password != "")
                    proxy.Credentials = new NetworkCredential(username, password);

                httpWebRequest.Proxy = proxy;
            }

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
        private static void LoadBrowser(string host = "", string service = "0", string username = "", string password = "")
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

                if (host != "" && service != "0")
                {
                    _browser = new ChromiumWebBrowser("about:blank", browserSettings, new RequestContext(new BrowserRequestContextHandler(host, service)));
                    if (username != "" && password != "")
                        _browser.RequestHandler = new BrowserRequestHandler(username, password);

                }
                else
                {
                    _browser = new ChromiumWebBrowser("about:blank", browserSettings, new RequestContext());
                }



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
                        _frameContent = resultHtml;
                        _endFrame = true;
                    });
                }
            }
        }
    }
}
