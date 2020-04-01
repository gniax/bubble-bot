using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BubbleBot.Core.Logs;
using BubbleBot.Utility.Extensions;
using BubbleBot.Core.Accounts.Network;
using BubbleBot.Core.Accounts.InGame;
using BubbleBot.Core.Enums;
using BubbleBot.Core.Commands;
using BubbleBot.Core.Accounts.Scripts;
using BubbleBot.Configurations;
using BubbleBot.Core.Accounts.Extensions;
using BubbleBot.Protocol.Messages;
using GalaSoft.MvvmLight;
using BubbleBot.Core.Accounts.Configurations;
using BubbleBot.Core.Accounts.Statistics;
using BubbleBot.Core.Groups;
using System.Diagnostics;
using BubbleBot.Utility;
using System.Dynamic;
using System.Threading;
using BubbleBot.Configurations.Language;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;
using System.Text;
using CefSharp.OffScreen;
using CefSharp;
using System.Runtime.CompilerServices;
using BubbleBot.Server.Messages;
using BubbleBot.Core.Extensions;
using System.Collections.Specialized;
using System.Linq;
using System.Globalization;

namespace BubbleBot.Core.Accounts
{
    public class Account : ViewModelBase, IEntity, IDisposable
    {

        // Fields
        private AccountStates _state;
        private bool _wasScriptRunning;
        private DateTime? _subscriptionEndDate;
        private bool _wasScriptEnabled;
        private string _apiKey = "";
        private string _token = "";
        private bool _fightLimitReached = false;

        public ChromiumWebBrowser browser;
        // Properties
        public static List<uint> AuthorizeByDefautTrade = new List<uint>();
        public static readonly SemaphoreSlim _AddSemaphore = new SemaphoreSlim(1,1);
        public AccountConfiguration AccountConfig { get; private set; }
        public Configuration Configuration { get; private set; }
        public FramesData FramesData { get; private set; }
        public string Token { get; private set; }
        public string Login { get; internal set; }
        public string GroupId { get; internal set; }
        public byte Group_Chief { get; internal set; }
        public DateTime? SubscriptionEndDate
        {
            get => _subscriptionEndDate;
            internal set => Set(ref _subscriptionEndDate, value);
        }
        public Logger Logger { get; private set; }
        public NetworkManager Network { get; private set; }
        public Game Game { get; private set; }
        public CommandsManager Commands { get; private set; }
        public ScriptsManager Scripts { get; private set; }
        public ExtensionsContainer Extensions { get; private set; }
        public StatisticsManager Statistics { get; private set; }
        public AccountStates State
        {
            get => _state;
            set
            {
                Set(ref _state, value);
                StateChanged?.Invoke();
            }
        }
        public Group Group { get; set; }
        public TimerWrapper PlanificationTimer { get; private set; }
        public bool IsBusy => State != AccountStates.NONE && State != AccountStates.REGENERATING;
        public Account Element => this;
        public bool HasGroup => Group != null;
        public bool IsGroupChief => !HasGroup || Group.Chief == this;
        public bool FightLimitReached
        {
            get => _fightLimitReached;
            set => Set(ref _fightLimitReached, value);
        }
        // This variable is used for auto-reconnection
        public bool IsIntentionalDisconnection = false;
        // This variable is used to prevent planfication reconnection (ex: when a bot is ban and others are disconnected or with function reconnect/disconnect)
        public bool PreventPlanificationReconnection = false;
        // Used to prevent auto-reconnection when it is impossible
        public bool PreventAutoReconnection = false;
        // Used to force restart script after captcha or a fight...
        public bool WaitForRestartScript = false;

        // Events
        public event Action StateChanged;
        public event Action<Account> RecaptchaReceived;
        public event Action<Account, bool> RecaptchaResolved;

        public static void addAutorizedPlayer(uint playerId)
        {
            _AddSemaphore.Wait();
            AuthorizeByDefautTrade.Add(playerId);
            _AddSemaphore.Release();
        }

        // Constructor
        public Account(AccountConfiguration accountConfig)
        {
            GroupId = "-";
            Group_Chief = 0;
            AccountConfig = accountConfig;
            State = AccountStates.DISCONNECTED;

            FramesData = new FramesData();
            Configuration = new Configuration(this);
            Logger = new Logger();
            Network = new NetworkManager(this);
            Game = new Game(this);
            Commands = new CommandsManager(this);
            Scripts = new ScriptsManager(this);
            Extensions = new ExtensionsContainer(this);
            Statistics = new StatisticsManager(this);
            PlanificationTimer = new TimerWrapper(30000, Planification_Callback);

            Network.Disconnected += Network_Disconnected;
            Game.Map.MapLoaded += Map_MapLoaded;
        }

        public async Task Connect()
        {
            if (!PlanificationTimer.Enabled)
                PlanificationTimer.Start();

            PreventAutoReconnection = false;
            FramesData.Clear();
            Network.Clear();
            Game.Clear();
            Extensions.Clear();
            Logger.LogInfo("", LanguageManager.Translate("10"));

            if (await SetToken())
            {
                State = AccountStates.CONNECTING;
                Logger.LogInfo("", LanguageManager.Translate("11"));
                await Network.ConnectToLoginServer();
            }
        }
        private void CloseBrowser()
        {
            if(browser != null)
            {
                if (browser.IsDisposed)
                {
                    browser = null;
                    return;
                }
                else if(browser.IsBrowserInitialized && browser.IsLoading)
                    browser.Stop();

                browser.RequestContext.Dispose();
                browser.Dispose();
                browser = null;
            }
        }
        private int returnKey(short method, object sender, FrameLoadEndEventArgs e)
        {
            // method : 1 => apikey
            // method : 2 => token
            if (e.Url.Contains("haapi")) //Si c'est une FRAME demander l'apikey
            {
                if (e.HttpStatusCode == 200) //Si la requète POST a fonctionné --> on continue 
                {
                    e.Frame.GetTextAsync().ContinueWith(taskHtml => //On lit ici la réponse
                    {
                        try
                        {
                            string html = taskHtml.Result; //réponse
                                                           //La réponse est en JSON, on en crée un dictionnaire
                            Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(html));

                            if (method == 1 && dictionaryRes.ContainsKey("key")) 
                            {
                                string apikey = (string)dictionaryRes["key"];
                                _apiKey = apikey;
                            }
                            else if(method == 2 && dictionaryRes.ContainsKey("token"))
                            {
                                string token = (string)dictionaryRes["token"];
                                _token = token;
                            }
                            return e.HttpStatusCode;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception : {0}", ex.Message);
                            if (method == 1)
                                _apiKey = "failed";
                            else if (method == 2)
                                _token = "failed";

                            return e.HttpStatusCode;
                        }
                    });

                }
                else if (e.HttpStatusCode == 601) //si c'est ban ou autre, on vérifie et on renvoie
                {
                    e.Frame.GetTextAsync().ContinueWith(taskHtml => 
                    {
                        string html = taskHtml.Result;
                        Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(html));
                        Logger.LogError("", dictionaryRes["reason"].ToString() == "BAN" ? LanguageManager.Translate("478") : LanguageManager.Translate("552"));
                        if (dictionaryRes["reason"].ToString() == "BAN")
                        {
                            // Auto reconnection
                            if (!AccountConfig.IsBan && GlobalConfiguration.Instance.AutomaticReconnection && !PreventAutoReconnection)
                            {
                                // Here we have to disconnect every bot which has set his auto disconnection
                                foreach (Account acc in BubbleBotMain.Instance.ConnectedAccounts)
                                {
                                    if (acc.Network.Connected && acc.Configuration.DisconnectOnBan && acc != this)
                                    {
                                        if (acc.Configuration.BanReconnectionDelay > 0)
                                        {
                                            acc.Logger.LogWarning(LanguageManager.Translate("654"), LanguageManager.Translate("653", this.Game.Character.Name, this.Game.Server.Name));
                                            acc.PreventPlanificationReconnection = true;
                                            acc.Reconnect(acc.Configuration.BanReconnectionDelay);
                                        }
                                        else
                                        {
                                            acc.Logger.LogWarning(LanguageManager.Translate("654"), LanguageManager.Translate("653", this.Game.Character.Name, this.Game.Server.Name));
                                            acc.PreventPlanificationReconnection = true;
                                            acc.Network.Disconnect("CLIENT_CLOSING", false).ConfigureAwait(false);
                                        }
                                    }
                                    else if(acc != this)
                                    {
                                        acc.Logger.LogWarning(LanguageManager.Translate("655"), LanguageManager.Translate("653", this.Game.Character.Name, this.Game.Server.Name));
                                    }
                                }
                            }
                            PreventPlanificationReconnection = true;
                            this.State = Enums.AccountStates.BANNED;
                            AccountConfig.IsBan = true;
                            GlobalConfiguration.Instance.Save();
                        }
                       
                        if (method == 1)
                            _apiKey = "failed";
                        else if (method == 2)
                            _token = "failed";
                    });
                   
                }
                else //Si la requête a été interdite .. à traiter : (HttpStatusCode... = 403)
                {
                    if(method == 1)
                        _apiKey = "failed";
                    else if(method ==2)
                        _token = "failed";
                    return e.HttpStatusCode;
                }
            }
            return e.HttpStatusCode;
        }

        async private Task SetProxy(ChromiumWebBrowser cwb, string Address)
        {
            if (browser != null && browser.IsBrowserInitialized)
            { 
                await Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    var rc = cwb.GetBrowser().GetHost().RequestContext;
                    var v = new Dictionary<string, object>();
                    v["mode"] = "fixed_servers";
                    v["server"] = Address;
                    string error;
                    bool success = rc.SetPreference("proxy", v, out error);
                });
            }
        }
        private async Task<bool> SetToken()
        {
            Console.WriteLine("[1/3] - Retrieving API key");
            string username = AccountConfig.Username;
            string password = AccountConfig.Password;

            CloseBrowser();

            //On charge le navigateur vide
            browser = new ChromiumWebBrowser("about:blank", null, new RequestContext());

            if (AccountConfig.Proxy.IsValid)
                await SetProxy(browser, (AccountConfig.Proxy.Ip + ':' + AccountConfig.Proxy.Port));

            //tant que le browser est pas initialisé on attend (tiemout 30sec)
            bool browserInit = System.Threading.SpinWait.SpinUntil(() => (browser.IsBrowserInitialized), TimeSpan.FromSeconds(20));

            if (!browserInit)
            {
                CloseBrowser();
                return false; //si le browser a pas chargé on annule
            }

            IFrame frame = browser.GetMainFrame();
            IRequest request = frame.CreateRequest();

            request.Url = "https://haapi.ankama.com/json/Ankama/v2/Api/CreateApiKey";
            byte[] bytes = Encoding.ASCII.GetBytes($"login={username}&password={password}&long_life_token=false");
            request.Method = "POST";

            request.InitializePostData();
            var element = request.PostData.CreatePostDataElement();
            element.Bytes = bytes;
            request.PostData.AddElement(element);
            frame.LoadRequest(request);

            //Quand elle est finit on traite le résultat dans une autre fonction (FrameLoadEnd)

            int httpCode = 0; //on récupère l'httpcode à titre informatif quand on va afficher l'erreur
            browser.FrameLoadEnd += delegate (object sender, FrameLoadEndEventArgs e)
            {
                httpCode = returnKey(1, RuntimeHelpers.GetObjectValue(sender), e);
            };

            // tant que apikey a pas changé on attend
            bool boolGetApiKey = System.Threading.SpinWait.SpinUntil(() => (_apiKey != ""), TimeSpan.FromSeconds(20));

            if (boolGetApiKey == false || _apiKey == "failed") //si au bout de 30 secondes l'apikey a pas de changement on annule / ou erreur
            {
                Logger.LogError("", LanguageManager.Translate("32", httpCode));
                CloseBrowser();
                _apiKey = "";
                return false;
            }

            Console.WriteLine("[2/3] - Retrieving account token");

            try
            {
                IFrame mainFrame = browser.GetMainFrame();
                IRequest tokenRequest = mainFrame.CreateRequest(initializePostData: false);
                tokenRequest.Url = "https://haapi.ankama.com/json/Ankama/v2/Account/CreateToken?game=18";
                tokenRequest.Method = "GET";
                tokenRequest.SetHeaderByName("apikey", _apiKey, overwrite: true);
                mainFrame.LoadRequest(tokenRequest);

                httpCode = 0;
                browser.FrameLoadEnd += delegate (object sender, FrameLoadEndEventArgs e)
                {
                    httpCode = returnKey(2, RuntimeHelpers.GetObjectValue(sender), e);
                };


                // tant que apikey a pas changé on attend
                bool getToken = System.Threading.SpinWait.SpinUntil(() => (_token != ""), TimeSpan.FromSeconds(20));

                if (getToken == false || _token == "failed") //si au bout de 30 secondes l'apikey a pas de changement on annule / ou erreur
                {
                    Logger.LogError("", LanguageManager.Translate("32", httpCode));
                    CloseBrowser();
                    _token = "";
                    _apiKey = "";
                    return false;
                }

                Console.WriteLine("[3/3] - Authenticated");
                _apiKey = "";
                Token = _token;
                _token = "";
                CloseBrowser();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur {0}", ex);
            }
            CloseBrowser();
            return false;
        }

        public async Task HandleRecaptcha(string sitekey, int tries = 1)
        {
            State = AccountStates.RECAPTCHA;
            // If _wasScriptRunning was already true, don't change it
            _wasScriptRunning = _wasScriptRunning || Scripts.Enabled;
            Scripts.StopScript();
            RecaptchaReceived?.Invoke(this);

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                RecaptchaHandler ByPassCaptcha = new RecaptchaHandler();
                Logger.LogDebug("reCaptcha", "Getting response..");
                string response = ByPassCaptcha.GetResponse(sitekey);
                Logger.LogDebug("reCaptcha", "Got response.");

                // If the response is null, its because the user didn't enter an anti-captcha key
                if (response == null)
                {
                    // We shouldn't leave this True
                    _wasScriptRunning = false;
                    Logger.LogError(LanguageManager.Translate("71"), LanguageManager.Translate("73"));
                    RecaptchaResolved?.Invoke(this, false);
                }
                else
                {
                    Logger.LogInfo(LanguageManager.Translate("71"), LanguageManager.Translate("74", sw.Elapsed.TotalSeconds));

                    dynamic msg = new ExpandoObject();
                    msg.call = "recaptchaResponse";
                    msg.data = response;
                    string raw = JsonConvert.SerializeObject(msg);

                    Logger.LogDebug(LanguageManager.Translate("71"), LanguageManager.Translate("75"));
                    await Network.SendRawAsync(raw);

                    if (State == AccountStates.RECAPTCHA)
                    {
                        State = AccountStates.NONE;
                    }

                    // Resume script if this is a solo account
                    // Sometimes this will fail if we receive more than one captcha
                    if (!HasGroup && (_wasScriptRunning || WaitForRestartScript))
                    {
                        await Task.Delay(2000);
                        Logger.LogDebug(LanguageManager.Translate("71"), LanguageManager.Translate("76"));
                        Scripts.StartScript();

                        // Only set reset _wasScriptRunning if the script was actually started
                        // Because if the bot received another recaptcha, StartScript will just return because IsBusy will be True
                        if (Scripts.Enabled)
                        {
                            WaitForRestartScript = false;
                            _wasScriptRunning = false;
                        }
                    }
                    // Otherwise if this is a group member, trigger RecaptchaResolved
                    else if (HasGroup)
                    {
                        RecaptchaResolved?.Invoke(this, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("reCaptcha", ex.Message);
                if (tries < 3)
                {
                    Logger.LogError("reCatpcha", LanguageManager.Translate("582", ++tries));
                    await HandleRecaptcha(sitekey, tries);
                }
            }
        }

        #region States Checking

        public bool IsFighting()
            => State == AccountStates.FIGHTING;
        public bool IsSubscribed()
            => SubscriptionEndDate.HasValue;
        public bool IsGathering()
            => State == AccountStates.GATHERING;
        public bool isFightLimitReached()
            => FightLimitReached;
        public bool IsInDialog()
            => State == AccountStates.STORAGE || State == AccountStates.TALKING || State == AccountStates.EXCHANGE || State == AccountStates.BUYING || State == AccountStates.SELLING;

        #endregion

        public void LeaveDialog()
        {
            if (IsInDialog())
            {
                Network.SendMessage(new LeaveDialogRequestMessage());
            }
        }

        private void Network_Disconnected(NetworkManager networkManager)
        {
            try
            {
                if(State != AccountStates.BANNED && !AccountConfig.IsBan)
                    State = AccountStates.DISCONNECTED;
                Logger.LogWarning("Network", LanguageManager.Translate("31"));

                CloseBrowser();

                // In case there was a script enabled
                if (Network.Phase != NetworkPhases.SWITCHING_TO_GAME)
                {
                    WaitForRestartScript = false;

                    BubbleBotMain.Instance.Server.SendMessage(new BotInformationsMessage(
                        AccountConfig.Username,
                        Game.Character.Level,
                        (byte)Game.Character.Stats.EnergyPercent,
                        (byte)Game.Character.Inventory.WeightPercent,
                        Game.Character.Inventory.Kamas,
                        Game.Map.Id,
                        Game.Map.CurrentPosition,
                        State.ToString(),
                        "-",
                        0,
                        Scripts.CurrentScriptName != null ? Scripts.CurrentScriptName : "-"
                    ));

                    _wasScriptEnabled = Scripts.Enabled;
                    Scripts.StopScript();
                    Extensions.Flood.Stop();
                    // In case the disconnection isnt intentional
                    if(!IsIntentionalDisconnection && !AccountConfig.IsBan && State != AccountStates.BANNED && !PreventAutoReconnection && GlobalConfiguration.Instance.AutomaticReconnection)
                    {
                        var task = Task.Run(async() =>
                        {
                            Logger.LogMessage(LanguageManager.Translate("12"), LanguageManager.Translate("616"));
                            await Connect().ConfigureAwait(true);
                            if(Network != null)
                            {
                                if (Network.Connected)
                                {
                                    WaitForRestartScript = true;
                                }
                                else
                                {
                                    Logger.LogMessage(LanguageManager.Translate("12"), LanguageManager.Translate("630"));
                                    IsIntentionalDisconnection = false;
                                    await Network.Disconnect("CLIENT_CLOSING", true).ConfigureAwait(true);
                                    Network_Disconnected(networkManager);
                                    return;
                                }                           
                            }
                        });
                    }
                }
                IsIntentionalDisconnection = false;
            } catch (Exception ex)
            {
                IsIntentionalDisconnection = false;
                Console.WriteLine("Exception ex: {0}", ex.Message);
            }
        }

        #region Reconnection
        public async void Reconnect(int Seconds)
        {
            DateTime localDate = DateTime.Now;
            DateTime newDate = localDate.AddSeconds(Seconds);

            string newDateToDay = newDate.Day.ToString() + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(newDate.Month);
            string newDateToTime = newDate.ToString("HH:mm:ss");

            Logger.LogMessage(LanguageManager.Translate("165"), LanguageManager.Translate("612", newDateToDay, newDateToTime, "automatique"));
            await Network.Disconnect("CLIENT_CLOSING");
            await Task.Delay(400);

            // Note: Here we'll log informations about current timer before reconnection
            // More the longer the time, and more will be display informations about situation
            // Ex: For 30 seconds reconnection -> 1 display at the half (show 15 seconds remaining)
            // For 86440s (1 day) -> a display each hour
            for (int i = 0; i < Seconds; i++)
            {
                int factor = 0;

                if (Seconds > 30 && Seconds <= 300)
                {
                    factor = 2;
                }
                else if (Seconds > 300 && Seconds <= 1800)
                {
                    factor = 3;
                }
                else if (Seconds > 1800 && Seconds <= 7200)
                {
                    factor = 5;
                }
                else if (Seconds > 7200 && Seconds <= 43200)
                {
                    factor = 8;
                }
                else
                {
                    factor = 12;
                }

                if (i == Seconds - 60 || Enumerable.Range(1, factor - 1).Any(n => i == (Seconds / factor * n)))
                {
                    TimeSpan time = TimeSpan.FromSeconds(Seconds - i);

                    string format = @"hh\:mm\:ss";

                    if ((Seconds - i) < 60)
                        format = @"ss";
                    else if ((Seconds - i) < 3600)
                        format = @"mm\:ss";

                    string timeDisplay = time.ToString(format);
                    Logger.LogMessage(LanguageManager.Translate("165"), LanguageManager.Translate("614", timeDisplay));
                }

                // Here set the delay to 1sec
                // Note: We get for 22 hours delay, 22h20 real delay 
                // it means the 1s function delay is longer than the right 1 sec -> we needs a coefficient to settle the timer 
                // 22h20 = 80400s && 22h = 79200s => 80400/79200 ~= 1.015 // 1000 / 1.015 ~= 985 
                // TODO -- check if this theorical calculation is suitable
                // update: there's a +4 secs offset every 1h20 => unable to fix it => 984 is not enough
                await Task.Delay(985);
            }

            if (Network.Connected)
                return;

            await Connect().ConfigureAwait(true);

            if(Network.Connected)
                WaitForRestartScript = true;             

            return;
        }
        #endregion

        // Note: this function is now used even if the planification is not activated.. read next comments
        private async void Planification_Callback(object state)
        {
            int hour = DateTime.Now.Hour;
            // [Planification Activated]
            // If the bot is connected and the hour is red 
            if (Network.Connected && AccountConfig.Planification[hour] == false && State != AccountStates.FIGHTING && AccountConfig.PlanificationActivated)
            {
                Logger.LogInfo("Planificateur", LanguageManager.Translate("584"));
                PreventAutoReconnection = false;
                PreventPlanificationReconnection = false;
                await Network.Disconnect("CLIENT_CLOSING");
            }
            // [Planification Activated]
            // If the bot is disconnected and the hour is green
            else if (State == AccountStates.DISCONNECTED && AccountConfig.Planification[hour] && !PreventPlanificationReconnection && AccountConfig.PlanificationActivated)
            {
                Logger.LogInfo("Planificateur", LanguageManager.Translate("585"));
                try
                {
                    await Connect();
                    if(AccountConfig.ForceStartScript)
                        WaitForRestartScript = true;
                }
                catch (Exception ex)
                {
                    Logger?.LogError("", ex.ToString());
                }
            }
            // [Either Planification Actived or Deactivated]
            // If the bot is connected and the script is not running as we want 
            else if(Network.Connected && !Scripts.Running && WaitForRestartScript && !IsBusy)
            {
                if ((HasGroup && IsGroupChief) || !HasGroup)
                    Scripts.StartScript();

                await Task.Delay(1500);

                if (Scripts.Enabled && Scripts.Running)
                    WaitForRestartScript = false;
            }
        }

        private async void Map_MapLoaded()
        {
            if(WaitForRestartScript)
            {
                // If this account is a group chief or solo, restart script
                if ((HasGroup && IsGroupChief) || !HasGroup)
                    Scripts.StartScript();

                await Task.Delay(1500);

                if(Scripts.Enabled && Scripts.Running)
                { 
                    WaitForRestartScript = false;
                }
                return;
            }

            if (Scripts.Running || !AccountConfig.PlanificationActivated || (!_wasScriptEnabled && !AccountConfig.ForceStartScript))
                return;

            await Task.Delay(1500);

            if(Scripts.CurrentScriptName != null)
                Logger.LogInfo("Planificateur", LanguageManager.Translate("583"));

            if ((HasGroup && IsGroupChief) || !HasGroup)
                Scripts.StartScript();
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            CloseBrowser();
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Logger.Dispose();
                    Network.Dispose();
                    Game.Dispose();
                    Scripts.Dispose();
                    Extensions.Dispose();
                    Configuration.Dispose();
                    Statistics.Dispose();
                    Commands.Dispose();
                    PlanificationTimer.Dispose();
                    if(browser != null && !browser.IsDisposed)
                    browser.Dispose();

                }

                browser = null;
                _state = AccountStates.NONE;
                _apiKey = "";
                _token = "";
                AccountConfig = null;
                Configuration = null;
                Token = null;
                Login = null;
                Logger = null;
                Network = null;
                Game = null;
                Scripts = null;
                Extensions = null;
                Statistics = null;
                FramesData = null;
                Commands = null;
                PlanificationTimer = null;

                _fightLimitReached = false;
                WaitForRestartScript = false;
                IsIntentionalDisconnection = false;
                PreventPlanificationReconnection = false;
                PreventAutoReconnection = false;
                _disposedValue = true;
            }
        }

        ~Account() => Dispose(false);

        public void Dispose() => Dispose(true);

        #endregion

    }
}