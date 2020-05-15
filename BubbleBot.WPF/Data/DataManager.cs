using BubbleBot.Configurations;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Data;
using BubbleBot.Utility.DofusTouch;
using CefSharp;
using CefSharp.OffScreen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BubbleBot.Data
{
    public class DataManager
    {

        // Fields
        private static Dictionary<string, ConcurrentDictionary<int, IData>> _cache;
        private static ChromiumWebBrowser _browser;
        private static SemaphoreSlim _sheduleDataRequest;
        public static void Initialize()
        {
            _cache = new Dictionary<string, ConcurrentDictionary<int, IData>>();
            _sheduleDataRequest = new SemaphoreSlim(1,1);
            LoadBrowser();

            var dataType = typeof(IData);

            //  .Net Core 1.1
            var assTypes = Assembly.GetAssembly(dataType).GetTypes();
            foreach (var type in assTypes)
            {
                if (!dataType.IsAssignableFrom(type) || type == dataType)
                    continue;

                _cache.Add(type.Name, new ConcurrentDictionary<int, IData>());
            }
        }

        public static async Task<T> Get<T>(int id) where T : IData
        {

            var Object = await GetOrDownload<T>(new[] { id });
            //Console.WriteLine("TESTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT");
            //Console.WriteLine("NULLLLLLLLLLLLLLLLLLLLL?" + Object.ElementAt(0).ToString());
           return Object.ElementAt(0);
        }

        public static async Task<IEnumerable<T>> GetEnumerableAsync<T>(IEnumerable<int> ids) where T : IData => await GetOrDownload<T>(ids);

        public static async Task<List<T>> GetListAsync<T>(IEnumerable<int> ids) where T : IData
        {
            var ObjectList = await GetOrDownload<T>(ids);
            return ObjectList.ToList();
        }

        private static async Task<IEnumerable<T>> GetOrDownload<T>(IEnumerable<int> ids) where T : IData
        {
           
            Stopwatch sw = Stopwatch.StartNew();
            string className = typeof(T).Name;
            string dir = Path.Combine(Directory.GetCurrentDirectory(), "Data", className);
            List<T> data = new List<T>();
            List<int> idsToDownload = new List<int>();
            foreach (int id in ids)
            {
                var cachedData = _cache[className].ContainsKey(id) ? _cache[className][id] : null;

                // If its already in cache
                if (cachedData != null)
                {
                    data.Add((T)cachedData);
                }
                else
                {
                    // If it exists as a file
                    string filePath = Path.Combine(dir, $"{id}.bbot");
                    if (File.Exists(filePath))
                    {
                        var dataEntry = (T)JsonConvert.DeserializeObject(File.ReadAllText(filePath), typeof(T));
                        // Cache it then add it to the list
                        _cache[className].TryAdd(dataEntry.Id, dataEntry);
                        data.Add(dataEntry);
                    }
                    // If not then we add it to the download list
                    else
                    {
                        idsToDownload.Add(id);
                    }
                }
            }

            if (idsToDownload.Count > 0)
            {
                try
                {
                    // This will download, cache and save all of the needed entries
                    data.AddRange(await Download<T>(idsToDownload));
                }
                catch
                {
                    // ignored
                }
            }

            Console.WriteLine($"Got {data.Count} entries in {sw.Elapsed.Milliseconds}ms.");
            return data;
        }

        private static async Task<IEnumerable<T>> Download<T>(IEnumerable<int> ids) where T : IData
        {
            string className = typeof(T).Name;
            List<KeyValuePair<string, string>> contentKvps = new List<KeyValuePair<string, string>>();

            IEnumerable<T> CacheAndSave<T>(IEnumerable<T> data) where T : IData
            {
                foreach (var entry in data)
                {
                    // Cache it
                    _cache[className].TryAdd(entry.Id, entry);
                    bool isWrite = false;
                    while(!isWrite)
                    {
                        try
                        {
                            // Then save it
                            var dir = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Data", className));
                            File.WriteAllText(Path.Combine(dir.FullName, $"{entry.Id}.bbot"), JsonConvert.SerializeObject(entry, Formatting.None));
                            isWrite = true;
                        }
                        catch
                        {
                            Console.WriteLine("Erreur Ouverture du fichier ");
                        }
                    }
                }

                return data;
            }


            string listIdPost = "";
            bool firstElement = true;

            foreach (var id in ids)
            {
                if (firstElement == true)
                {
                    listIdPost += id.ToString();
                    firstElement = false;
                }
                else
                {
                    listIdPost += ", " + id.ToString();
                }
            }

            string dataToPost = "{class: " + '\'' + className + '\'' + ", ids: [" + listIdPost + "]}";

            /*     var mainFrame = _browser.GetMainFrame();
                 var dataRequest = mainFrame.CreateRequest();
                 dataRequest.Url = $"https://proxyconnection.touch.dofus.com/data/map?lang={GlobalConfiguration.Instance.Lang}&v={DTConstants.AssetsVersion}";
                 dataRequest.SetHeaderByName("accept-encoding", "gzip, deflate, br", true);
                 dataRequest.SetHeaderByName("accept-language", "fr", true);
                 dataRequest.SetHeaderByName("user-agent", "Mozilla/5.0 (Linux; Android 7.1.1; K92 Build/NMF26V; wv) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.124 Mobile Safari/537.36", true);
               */
            //  dataRequest.SetHeaderByName("accept", "*/*", true);

            // var bytes = Encoding.ASCII.GetBytes(bytesArray);

            //await _browser.GetMainFrame().EvaluateScriptAsync("var request = new XMLHttpRequest();");
            //Initialisation des valeurs de la requête 
           // _sheduleDataRequest.Wait();

            string reqSalt = GetRandomString(20);
            await _browser.GetMainFrame().EvaluateScriptAsync("var params"+ reqSalt + " = " + dataToPost + ";");
            await _browser.GetMainFrame().EvaluateScriptAsync("var xhr" + reqSalt + " = new XMLHttpRequest();");
            await _browser.GetMainFrame().EvaluateScriptAsync("xhr" + reqSalt + ".open('POST', 'https://proxyconnection.touch.dofus.com/data/map?lang=fr&v=1.46.9', false);");
            //await Task.Delay(500);
            await _browser.GetMainFrame().EvaluateScriptAsync("xhr" + reqSalt + ".setRequestHeader('content-type', 'application/json; charset=UTF-8');");
            await _browser.GetMainFrame().EvaluateScriptAsync("xhr" + reqSalt + ".send(JSON.stringify(params" + reqSalt + "));");
            //await Task.Delay(500);
            //await Task.Delay(50);

            JavascriptResponse takeInfo100 = await _browser.GetMainFrame().EvaluateScriptAsync("xhr" + reqSalt + ".response;");
            string resultCreate = JsonConvert.SerializeObject(takeInfo100.Result);
            Console.WriteLine("Resultat requête NON TRAITER:" + resultCreate);
            string specChar = string.Format("{0}{1}{2}{3}", @"\", @"\", @"\", "\"");
            string resultwork = resultCreate.Replace(specChar, "'");
            string resultClean = resultwork.Replace(@"\", "");
            Console.WriteLine("Resultat requête :" + resultClean.Substring(1, resultClean.Length - 2));
            //_sheduleDataRequest.Release();

          try
          {
              var dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(resultClean.Substring(1, resultClean.Length - 2));
              // In case no objects were found
              if (dict.Count == 0)
                  return new List<T>() { default(T)};

                var data = dict.Values.Select(f => (T)f.ToObject(typeof(T))).ToList();

                return CacheAndSave(data);
          }
          catch { return null; }
}
        private static Random random2 = new Random();
        private static string GetRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random2.Next(s.Length)]).ToArray());
        }


        /*         dataRequest.Method = "POST";
                 dataRequest.InitializePostData();
                 var element = dataRequest.PostData.CreatePostDataElement();
                 element.Bytes = bytes;
                 dataRequest.PostData.AddElement(element);
                 mainFrame.LoadRequest(dataRequest);

                 string responseString = null;
                 int i = 0;
                 _browser.FrameLoadEnd += delegate (object sender, FrameLoadEndEventArgs e)
                 {
                     i++;
                     GetContent(RuntimeHelpers.GetObjectValue(sender), e);
                     Console.WriteLine("Recept :" + i.ToString());
                 };

                 var endFrameLoad = SpinWait.SpinUntil(() => endFrame != false, TimeSpan.FromSeconds(20));
                 endFrame = false;
                 responseString = resultString;
                 Console.WriteLine("SHEDULE : " + responseString);
                 */


        /*      try
              {
                  var dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(responseString);
                  // In case no objects were found
                  if (dict.Count == 0)
                      return new List<T>() { default(T)
  };

  var data = dict.Values.Select(f => (T)f.ToObject(typeof(T))).ToList();

                  return CacheAndSave(data);
              }
              catch { return null; }
              */

        /*
                private static bool endFrame = false;
                private static string resultString = null;
                private static IEnumerable<T> Download<T>(IEnumerable<int> ids) where T : IData
                {
                    string className = typeof(T).Name;
                    List<KeyValuePair<string, string>> contentKvps = new List<KeyValuePair<string, string>>();

                    IEnumerable<T> CacheAndSave<T>(IEnumerable<T> data) where T : IData
                    {
                        foreach (var entry in data)
                        {
                            // Cache it
                            _cache[className].TryAdd(entry.Id, entry);

                            // Then save it
                            var dir = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Data", className));
                            File.WriteAllText(Path.Combine(dir.FullName, $"{entry.Id}.bbot"), JsonConvert.SerializeObject(entry, Formatting.None));
                        }

                        return data;
                    }

                    contentKvps.Add(new KeyValuePair<string, string>("class", className));
                    string bytesArray = "class=" + className;
                    foreach (var id in ids)
                    {
                        contentKvps.Add(new KeyValuePair<string, string>("ids[]", id.ToString()));
                        bytesArray += "&ids[]=" + id.ToString();
                    }

                    var mainFrame = _browser.GetMainFrame();
                    var dataRequest = mainFrame.CreateRequest();
                    dataRequest.Url = $"https://proxyconnection.touch.dofus.com/data/map?lang={GlobalConfiguration.Instance.Lang}&v={DTConstants.AssetsVersion}";
                    dataRequest.SetHeaderByName("accept-encoding", "gzip, deflate, br", true);
                    dataRequest.SetHeaderByName("accept-language", "fr", true);
                    dataRequest.SetHeaderByName("user-agent", "Mozilla/5.0 (Linux; Android 7.1.1; K92 Build/NMF26V; wv) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.124 Mobile Safari/537.36", true);
            */
        //dataRequest.SetHeaderByName("accept", "*/*", true);
        /*
                var bytes = Encoding.ASCII.GetBytes(bytesArray);


                       dataRequest.Method = "POST";
                         dataRequest.InitializePostData();
                         var element = dataRequest.PostData.CreatePostDataElement();
                         element.Bytes = bytes;
                         dataRequest.PostData.AddElement(element);
                         mainFrame.LoadRequest(dataRequest);

                         string responseString = null;
                         int i = 0;
                         _browser.FrameLoadEnd += delegate (object sender, FrameLoadEndEventArgs e)
                         {
                             i++;
                             GetContent(RuntimeHelpers.GetObjectValue(sender), e);
                             Console.WriteLine("Recept :" + i.ToString());
                         };

                         var endFrameLoad = SpinWait.SpinUntil(() => endFrame != false, TimeSpan.FromSeconds(20));
                         endFrame = false;
                         responseString = resultString;
                         Console.WriteLine("SHEDULE : " + responseString);



                try
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(responseString);
                    // In case no objects were found
                    if (dict.Count == 0)
                        return new List<T>() { default(T) };

                    var data = dict.Values.Select(f => (T)f.ToObject(typeof(T))).ToList();

                    return CacheAndSave(data);
                }
                catch { return null; }

            }
    *//*
            public static void GetContent(object sender, FrameLoadEndEventArgs e)
            {
                if (e.Frame.IsMain)
                {
                    if (e.HttpStatusCode == 200)
                    {
                        e.Frame.GetTextAsync().ContinueWith(taskHtml =>
                        {
                            var resultHtml = taskHtml.Result;
                            resultString = resultHtml;
                            Console.WriteLine("keep : " + resultString);
                            endFrame = true;
                        });
                    }
                }
            }
            */
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
                    WindowlessFrameRate = 1,
            };


                _browser = new ChromiumWebBrowser("about:blank", browserSettings, new RequestContext(new BrowserRequestContextHandler("api.example.com", "45785")));
                _browser.RequestHandler = new BrowserRequestHandler("Selmistonifer9318", "T7k4VcH");


                //_browser = new ChromiumWebBrowser("about:blank", browserSettings, new RequestContext());


                var browserInit = SpinWait.SpinUntil(() => _browser.IsBrowserInitialized, TimeSpan.FromSeconds(20));
            }
        }

    }
}
