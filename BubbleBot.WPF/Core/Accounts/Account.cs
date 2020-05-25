using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.Configurations;
using BubbleBot.Core.Accounts.Extensions;
using BubbleBot.Core.Accounts.InGame;
using BubbleBot.Core.Accounts.Network;
using BubbleBot.Core.Accounts.Scripts;
using BubbleBot.Core.Accounts.Statistics;
using BubbleBot.Core.Commands;
using BubbleBot.Core.Enums;
using BubbleBot.Core.Groups;
using BubbleBot.Core.Logs;
using BubbleBot.Data;
using BubbleBot.Protocol.Messages;
using BubbleBot.Utility;
using CefSharp;
using CefSharp.OffScreen;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace BubbleBot.Core.Accounts
{
    public class Account : ViewModelBase, IEntity, IDisposable
    {
        private string _apiKey;
        private bool _fightLimitReached;

        // Fields
        private AccountStates _state;
        private DateTime? _subscriptionEndDate;
        private CancellationTokenSource _taskCancelToken;
        private string _token;
        private bool _wasScriptEnabled;
        private bool _wasScriptRunning;

        public ChromiumWebBrowser Browser;

        public string SidResponse;

        // This variable is used for auto-reconnection
        public bool IsIntentionalDisconnection;

        // Used to prevent auto-reconnection when it is impossible
        public bool PreventAutoReconnection;

        // This variable is used to prevent planfication reconnection (ex: when a bot is ban and others are disconnected or with function reconnect/disconnect)
        public bool PreventPlanificationReconnection;

        // Used to force restart script after captcha or a fight...
        public bool WaitForRestartScript;

        // Constructor
        public Account(AccountConfiguration accountConfig)
        {
            GroupId = "-";
            Group_Chief = 0;
            AccountConfig = accountConfig;
            State = AccountStates.DISCONNECTED;

            FramesData = new FramesData();
            Configuration = new Configuration(this);
            Logger = new Logger(this);
            Network = new NetworkManager(this);
            Game = new Game(this);
            Commands = new CommandsManager(this);
            Scripts = new ScriptsManager(this);
            Extensions = new ExtensionsContainer(this);
            Statistics = new StatisticsManager(this);
            PlanificationTimer = new TimerWrapper(30000, Planification_Callback);
            //DataManager = new DataManager(this);

            Network.Disconnected += Network_Disconnected;
            Game.Map.MapLoaded += Map_MapLoaded;
        }

        // Properties
        public AccountConfiguration AccountConfig { get; private set; }
        //public DataManager DataManager { get; private set; }
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
        public KeyValuePair<string, DateTime> ConnectError { get; set; }
        public bool HasGroup => Group != null;
        public bool IsGroupChief => !HasGroup || Group.Chief == this;

        public bool FightLimitReached
        {
            get => _fightLimitReached;
            set => Set(ref _fightLimitReached, value);
        }

        // Events
        public event Action StateChanged;
        public event Action<Account> RecaptchaReceived;
        public event Action<Account, bool> RecaptchaResolved;

        public async Task Connect()
        {
            await Task.Run(async () =>
            {
                if (State != AccountStates.DISCONNECTED)
                    return;

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
            });
        }
        public bool LoadBrowser()
        {
            if (Browser == null || Browser.IsDisposed)
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
                
                if (AccountConfig.Proxy.IsValid)
                {
                    Browser = new ChromiumWebBrowser("about:blank", browserSettings,
                        new RequestContext(new BrowserRequestContextHandler(AccountConfig.Proxy.Ip, AccountConfig.Proxy.Port.ToString())));
                    Browser.RequestHandler =
                        new BrowserRequestHandler(AccountConfig.Proxy.Username, AccountConfig.Proxy.Password);
                }
                else
                {
                    if (GlobalConfiguration.Instance.IsProxyValid)
                    {
                        Browser = new ChromiumWebBrowser("about:blank", browserSettings, new RequestContext(new BrowserRequestContextHandler(GlobalConfiguration.Instance.ProxyIp, GlobalConfiguration.Instance.ProxyPort.ToString())));
                        Browser.RequestHandler = new BrowserRequestHandler(GlobalConfiguration.Instance.ProxyUsername, GlobalConfiguration.Instance.ProxyPassword);
                    }
                    else
                    {
                        Browser = new ChromiumWebBrowser("about:blank", browserSettings, new RequestContext());
                    }
                }

                var browserInit = SpinWait.SpinUntil(() => Browser.IsBrowserInitialized, TimeSpan.FromSeconds(20));

                if (!browserInit)
                {
                    CloseBrowser();
                    return false;
                }

                return true;
            }
            return true;
        }

        public void CloseBrowser()
        {
            if (Browser != null)
            {
                if (Browser.IsDisposed)
                {
                    Browser = null;
                    return;
                }

                if (Browser.IsBrowserInitialized && Browser.IsLoading) Browser?.Stop();

                Browser?.Dispose();
                Browser?.RequestContext?.Dispose();
                Browser = null;
            }
        }

        public int SetKey(short method, object sender, FrameLoadEndEventArgs e)
        {
            // method : 1 => apikey
            // method : 2 => token
            // method : 3 => sid 
            // 200 => Success => Read Token or Apikeycor Sid
            // 601 => Ban => Disconnect other accounts if needed
            // 429 => Too Many Requests => Retry after 

            if ((e.Url.Contains("haapi") || e.Url.Contains("touch.dofus.com")) && SidResponse == null)
            {
                if (e.HttpStatusCode == 200) 
                {
                    _taskCancelToken = new CancellationTokenSource();
                    e.Frame.GetTextAsync().ContinueWith(taskHtml =>
                    {
                        var resultHtml = taskHtml.Result;

                        resultHtml = resultHtml.Substring(resultHtml.IndexOf("{"));

                        var dictionaryRes =
                            JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(resultHtml));

                        if (method == 1 && dictionaryRes.ContainsKey("key"))
                        {
                            Logger.LogInfo("", LanguageManager.Translate("730", (string)dictionaryRes["ip"]));
                            _apiKey = (string)dictionaryRes["key"];
                            _taskCancelToken.Cancel(false);
                        }
                        else if (method == 2 && dictionaryRes.ContainsKey("token"))
                        {
                            _token = (string)dictionaryRes["token"];
                            _taskCancelToken.Cancel(false);
                        }
                        else if (method == 3)
                        {
                            SidResponse = (string)dictionaryRes["sid"];
                            Network?.SetWebsocketTimer((long) dictionaryRes["pingInterval"],
                                (long) dictionaryRes["pingTimeout"]);
                            _taskCancelToken.Cancel(false);
                        }
                    }, _taskCancelToken.Token);
                    return e.HttpStatusCode;
                }

                if (e.HttpStatusCode == 601)
                {
                    e.Frame.GetTextAsync().ContinueWith(taskHtml =>
                    {
                        var html = taskHtml.Result;
                        var dictionaryRes =
                            JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(html));
                        Logger.LogError("",
                            dictionaryRes["reason"].ToString() == "BAN"
                                ? LanguageManager.Translate("478")
                                : LanguageManager.Translate("552"));
                        if (dictionaryRes["reason"].ToString() == "BAN")
                        {
                            // Auto disconnect
                            // Note : if the account has auto reconnection and tries to reconnect and is ban, we disconnect every bots
                            // it is the only case we have to kick others bots
                            if (!AccountConfig.IsBan && GlobalConfiguration.Instance.AutomaticReconnection &&
                                !PreventAutoReconnection && Game.Character != null && Game.Character.IsSelected)
                                // Here we have to disconnect every bot which has set his auto disconnection
                                foreach (var acc in BubbleBotMain.Instance.ConnectedAccounts)
                                    if (acc.Network.Connected && acc.Configuration.DisconnectOnBan && acc != this)
                                    {
                                        acc.Logger.LogWarning(LanguageManager.Translate("654"),
                                            LanguageManager.Translate("653", Game.Character.Name, Game.Server.Name));
                                        acc.PreventPlanificationReconnection = true;
                                        if (acc.Configuration.BanReconnectionDelay > 0)
                                            acc.Reconnect(acc.Configuration.BanReconnectionDelay);
                                        else
                                            acc.Network.Disconnect("CLIENT_CLOSING").ConfigureAwait(false);
                                    }
                                    else if (acc != this)
                                    {
                                        acc.Logger.LogWarning(LanguageManager.Translate("654"),
                                            LanguageManager.Translate("655", Game.Character.Name, Game.Server.Name));
                                    }

                            PreventPlanificationReconnection = true;
                            State = AccountStates.BANNED;
                            AccountConfig.IsBan = true;
                            GlobalConfiguration.Instance.Save();
                        }
                    });
                }
                else if (e.HttpStatusCode == 429)
                {
                    // Prevent reset timer at each time
                    if (ConnectError.Key != "Retry-After")
                        ConnectError = new KeyValuePair<string, DateTime>("Retry-After", DateTime.Now);
                }

                if (method == 1)
                    _apiKey = "failed";
                else if (method == 2)
                    _token = "failed";
                else if (method == 3)
                    SidResponse = "failed";
            }

            return e.HttpStatusCode;
        }

        private async Task<bool> SetToken()
        {
            await Task.Delay(1);

            Console.WriteLine("[1/3] - Retrieving API key");
            var username = AccountConfig.Username;
            var password = AccountConfig.Password;

            CloseBrowser();

            //On charge le navigateur vide

            LoadBrowser();

            var frame = Browser.GetMainFrame();
            var request = frame.CreateRequest();

            request.Url = "https://haapi.ankama.com/json/Ankama/v2/Api/CreateApiKey";
            var bytes = Encoding.ASCII.GetBytes($"login={username}&password={password}&long_life_token=false");
            request.Method = "POST";

            request.InitializePostData();
            var element = request.PostData.CreatePostDataElement();
            element.Bytes = bytes;
            request.PostData.AddElement(element);
            frame.LoadRequest(request);

            var httpCode = 0;
            Browser.FrameLoadEnd += delegate(object sender, FrameLoadEndEventArgs e)
            {
                httpCode = SetKey(1, RuntimeHelpers.GetObjectValue(sender), e);
            };

            var boolGetApiKey = SpinWait.SpinUntil(() => _apiKey != null, TimeSpan.FromSeconds(20));

            if (ConnectError.Key == "Retry-After")
            {
                var timeLeft = Math.Floor((ConnectError.Value.AddMinutes(10) - DateTime.Now).TotalSeconds);
                Logger.LogError(LanguageManager.Translate("12"), LanguageManager.Translate("670"));
                if (AccountConfig.PlanificationActivated)
                    Logger.LogError(LanguageManager.Translate("12"), LanguageManager.Translate("614", timeLeft));
                else
                    Logger.LogError(LanguageManager.Translate("12"), LanguageManager.Translate("671", timeLeft));
            }

            if (boolGetApiKey == false || _apiKey == "failed")
            {
                if (httpCode == 0 && (AccountConfig.Proxy.IsValid || (!AccountConfig.Proxy.IsValid && GlobalConfiguration.Instance.IsProxyValid)))
                    Logger.LogError("", LanguageManager.Translate("672"));

                else if (httpCode == 0)
                    Logger.LogError("", LanguageManager.Translate("673"));

                if (ConnectError.Key != "Retry-After" && httpCode != 0)
                    Logger.LogError("", LanguageManager.Translate("32", httpCode));
                CloseBrowser();
                _apiKey = null;
                return false;
            }

            Console.WriteLine("[2/3] - Retrieving account token");

            var mainFrame = Browser.GetMainFrame();
            var tokenRequest = mainFrame.CreateRequest(false);
            tokenRequest.Url = "https://haapi.ankama.com/json/Ankama/v2/Account/CreateToken?game=18";
            tokenRequest.Method = "GET";
            tokenRequest.SetHeaderByName("apikey", _apiKey, true);
            mainFrame.LoadRequest(tokenRequest);

            httpCode = 0;
            Browser.FrameLoadEnd += delegate(object sender, FrameLoadEndEventArgs e)
            {
                httpCode = SetKey(2, RuntimeHelpers.GetObjectValue(sender), e);
            };
           
            var getToken = SpinWait.SpinUntil(() => _token != null, TimeSpan.FromSeconds(20));

            if (ConnectError.Key == "Retry-After")
            {
                var timeLeft = Math.Floor((ConnectError.Value.AddMinutes(10) - DateTime.Now).TotalSeconds);
                Logger.LogError(LanguageManager.Translate("12"), LanguageManager.Translate("670"));
                if (AccountConfig.PlanificationActivated)
                    Logger.LogError(LanguageManager.Translate("12"), LanguageManager.Translate("614", timeLeft));
                else
                    Logger.LogError(LanguageManager.Translate("12"), LanguageManager.Translate("671", timeLeft));
            }

            if (getToken == false || _token == "failed")
            {
                if (httpCode == 0 && (AccountConfig.Proxy.IsValid || (!AccountConfig.Proxy.IsValid && GlobalConfiguration.Instance.IsProxyValid)))
                    Logger.LogError("", LanguageManager.Translate("672"));

                else if (httpCode == 0)
                    Logger.LogError("", LanguageManager.Translate("673"));

                if (ConnectError.Key != "Retry-After" && httpCode != 0)
                    Logger.LogError("", LanguageManager.Translate("32", httpCode));
                CloseBrowser();
                _token = null;
                _apiKey = null;
                return false;
            }

            Console.WriteLine("[3/3] - Success retrieving Token");

            Token = _token;
            _token = null;
            _apiKey = null;
            ConnectError = default;
            return true;
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
                var sw = Stopwatch.StartNew();
                var ByPassCaptcha = new RecaptchaHandler();
                Logger.LogDebug("reCaptcha", "Getting response..");
                var response = ByPassCaptcha.GetResponse(sitekey);
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
                    Logger.LogInfo(LanguageManager.Translate("71"),
                        LanguageManager.Translate("74", sw.Elapsed.TotalSeconds));

                    dynamic msg = new ExpandoObject();
                    msg.call = "recaptchaResponse";
                    msg.data = response;
                    string raw = JsonConvert.SerializeObject(msg);

                    Logger.LogDebug(LanguageManager.Translate("71"), LanguageManager.Translate("75"));
                    await Network.SendRawAsync(raw);

                    if (State == AccountStates.RECAPTCHA) State = AccountStates.NONE;

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

        public void LeaveDialog()
        {
            if (IsInDialog()) Network.SendMessage(new LeaveDialogRequestMessage());
        }

        private async void Network_Disconnected(NetworkManager networkManager)
        {
            try
            {
                if (State != AccountStates.BANNED && !AccountConfig.IsBan)
                    State = AccountStates.DISCONNECTED;
                Logger.LogWarning("Network", LanguageManager.Translate("31"));

                // In case there was a script enabled
                if (Network.Phase != NetworkPhases.SWITCHING_TO_GAME)
                {
                    WaitForRestartScript = false;
                    _wasScriptEnabled = Scripts.Enabled;
                    Scripts.StopScript();
                    Extensions.Flood.Stop();
                    // In case the disconnection isnt intentional
                    if (!IsIntentionalDisconnection && !AccountConfig.IsBan && State != AccountStates.BANNED &&
                        !PreventAutoReconnection &&
                        GlobalConfiguration.Instance.AutomaticReconnection)
                    {
                        var hour = DateTime.Now.Hour;
                        if (AccountConfig.PlanificationActivated && AccountConfig.Planification[hour] == false)
                            return;

                        Logger.LogMessage(LanguageManager.Translate("12"), LanguageManager.Translate("616"));

                        if (Network.ConnectTimeout != null)
                            Network.ConnectTimeout.Change(Timeout.Infinite, Timeout.Infinite);
                        PreventPlanificationReconnection = true;

                        Logger.LogMessage(LanguageManager.Translate("12"), LanguageManager.Translate("614", 60));
                        await Task.Delay(60000).ConfigureAwait(false);

                        if (State != AccountStates.CONNECTING)
                            await Connect().ConfigureAwait(false);
                        PreventPlanificationReconnection = false;
                        if (_wasScriptEnabled || _wasScriptRunning)
                            WaitForRestartScript = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception ex: {0}", ex.Message);
            }

            IsIntentionalDisconnection = false;
        }

        #region Reconnection

        public async void Reconnect(int Seconds)
        {
            var localDate = DateTime.Now;
            var newDate = localDate.AddSeconds(Seconds);

            var newDateToDay =
                newDate.Day + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(newDate.Month);
            var newDateToTime = newDate.ToString("HH:mm:ss");

            Logger.LogMessage(LanguageManager.Translate("165"),
                LanguageManager.Translate("612", newDateToDay, newDateToTime, "automatique"));
            await Network.Disconnect("CLIENT_CLOSING");
            await Task.Delay(400);

            // Note: Here we'll log informations about current timer before reconnection
            // More the longer the time, and more will be display informations about situation
            // Ex: For 30 seconds reconnection -> 1 display at the half (show 15 seconds remaining)
            // For 86440s (1 day) -> a display each hour
            for (var i = 0; i < Seconds; i++)
            {
                var factor = 0;

                if (Seconds > 30 && Seconds <= 300)
                    factor = 2;
                else if (Seconds > 300 && Seconds <= 1800)
                    factor = 3;
                else if (Seconds > 1800 && Seconds <= 7200)
                    factor = 5;
                else if (Seconds > 7200 && Seconds <= 43200)
                    factor = 8;
                else
                    factor = 12;

                if (i == Seconds - 60 || Enumerable.Range(1, factor - 1).Any(n => i == Seconds / factor * n))
                {
                    var time = TimeSpan.FromSeconds(Seconds - i);

                    var format = @"hh\:mm\:ss";

                    if (Seconds - i < 60)
                        format = @"ss";
                    else if (Seconds - i < 3600)
                        format = @"mm\:ss";

                    var timeDisplay = time.ToString(format);
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

            if (Network.Connected)
                WaitForRestartScript = true;
        }

        #endregion

        // Note: this function is now used even if the planification is not activated.. read next comments
        private async void Planification_Callback(object state)
        {
            var hour = DateTime.Now.Hour;
            // [Planification Activated]
            // If the bot is connected and the hour is red 
            if (Network.Connected && AccountConfig.Planification[hour] == false && State != AccountStates.FIGHTING &&
                AccountConfig.PlanificationActivated)
            {
                Logger.LogInfo("Planificateur", LanguageManager.Translate("584"));
                PreventAutoReconnection = true;
                PreventPlanificationReconnection = false;
                await Network.Disconnect("CLIENT_CLOSING");
            }
            // [Planification Activated]
            // If the bot is disconnected and the hour is green
            else if (State == AccountStates.DISCONNECTED && AccountConfig.Planification[hour] &&
                     !PreventPlanificationReconnection &&
                     AccountConfig.PlanificationActivated)
            {
                if (ConnectError.Value.AddMinutes(10) < DateTime.Now && State != AccountStates.CONNECTING)
                {
                    Logger.LogInfo("Planificateur", LanguageManager.Translate("585"));
                    try
                    {
                        await Connect();
                        if (AccountConfig.ForceStartScript)
                            WaitForRestartScript = true;
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError("", ex.ToString());
                    }

                    ConnectError = default;
                }
            }
            // [Planification Activated]
            // If the bot is disconnected after prevent planification
            else if (State == AccountStates.DISCONNECTED && AccountConfig.Planification[hour] == false &&
                     PreventPlanificationReconnection && AccountConfig.PlanificationActivated)
            {
                PreventPlanificationReconnection = false;
            }
            // [Either Planification Actived or Deactivated]
            // If the bot is connected and the script is not running as we want 
            else if (Network.Connected && !Scripts.Running && WaitForRestartScript && !IsBusy)
            {
                if (HasGroup && IsGroupChief || !HasGroup)
                    Scripts.StartScript();

                await Task.Delay(1500);

                if (Scripts.Enabled && Scripts.Running)
                    WaitForRestartScript = false;
            }
        }

        private async void Map_MapLoaded()
        {
            if (WaitForRestartScript)
            {
                // If this account is a group chief or solo, restart script
                if (HasGroup && IsGroupChief || !HasGroup)
                    Scripts?.StartScript();

                await Task.Delay(1500);

                if (Scripts.Enabled && Scripts.Running) WaitForRestartScript = false;
                return;
            }

            if (Scripts.Running || !AccountConfig.PlanificationActivated ||
                !_wasScriptEnabled && !AccountConfig.ForceStartScript)
                return;

            await Task.Delay(1500);

            if (Scripts?.CurrentScriptName != null)
                Logger.LogInfo("Planificateur", LanguageManager.Translate("583"));

            if (HasGroup && IsGroupChief || !HasGroup)
                Scripts?.StartScript();
        }

        #region States Checking

        public bool IsFighting()
        {
            return State == AccountStates.FIGHTING;
        }

        public bool IsSubscribed()
        {
            return SubscriptionEndDate.HasValue;
        }

        public bool IsGathering()
        {
            return State == AccountStates.GATHERING;
        }

        public bool isFightLimitReached()
        {
            return FightLimitReached;
        }

        public bool IsInDialog()
        {
            return State == AccountStates.STORAGE || State == AccountStates.TALKING ||
                   State == AccountStates.EXCHANGE || State == AccountStates.BUYING || State == AccountStates.SELLING;
        }

        #endregion

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _taskCancelToken?.Cancel(false);
                if (disposing)
                {
                    _taskCancelToken?.Dispose();
                    Logger?.Dispose();
                    Network?.Dispose();
                    Game?.Dispose();
                    Scripts?.Dispose();
                    Extensions?.Dispose();
                    Configuration?.Dispose();
                    Statistics?.Dispose();
                    Commands?.Dispose();
                    PlanificationTimer?.Dispose();
                    CloseBrowser();
                }

                Browser = null;
                _taskCancelToken = null;
                _state = AccountStates.NONE;
                _apiKey = null;
                _token = null;
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

        ~Account()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}