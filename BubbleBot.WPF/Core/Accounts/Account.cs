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

namespace BubbleBot.Core.Accounts
{
    public class Account : ViewModelBase, IEntity, IDisposable
    {

        // Fields
        private AccountStates _state;
        private bool _wasScriptRunning;
        private DateTime? _subscriptionEndDate;
        private bool _wasScriptEnabled;
        private ChromiumWebBrowser browser;
        private string _apiKey = "";
        private bool _fightLimitReached = false;

        // Properties
        public static List<uint> AuthorizeByDefautTrade = new List<uint>();
        public static readonly SemaphoreSlim _AddSemaphore = new SemaphoreSlim(1,1);
        public AccountConfiguration AccountConfig { get; private set; }
        public Configuration Configuration { get; private set; }
        public FramesData FramesData { get; private set; }
        public string Token { get; private set; }
        public string Login { get; internal set; }
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
        public bool IsIntentionalDisconnection = false;
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

        private int returnApiKey(object sender, FrameLoadEndEventArgs e)
        {
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

                            if (dictionaryRes.ContainsKey("key")) //On récupère l'apikey
                            {
                                string apikey = (string)dictionaryRes["key"];
                                _apiKey = apikey;
                            }
                            return e.HttpStatusCode;
                        }
                        catch (Exception ex)
                        {
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
                        if(dictionaryRes["reason"].ToString() == "BAN") this.State = Enums.AccountStates.BANNED;
                        _apiKey = "failed";
                    });
                   
                }
                else //Si la requête a été interdite .. à traiter : (HttpStatusCode... = 403)
                {
                    _apiKey = "failed";
                    return e.HttpStatusCode;
                }
            }
            return e.HttpStatusCode;
        }

        private async Task<bool> SetToken()
        {       
            Console.WriteLine("[1/3] - Retrieving API key");
            string username = AccountConfig.Username;
            string password = AccountConfig.Password;

            byte[] bytes = Encoding.ASCII.GetBytes($"login={username}&password={password}&long_life_token=false");

            //On charge le navigateur vide
            browser = new CefSharp.OffScreen.ChromiumWebBrowser("about:blank");

            //tant que le browser est pas initialisé on attend (tiemout 30sec)
            bool browserInit = System.Threading.SpinWait.SpinUntil(() => (browser.IsBrowserInitialized), TimeSpan.FromSeconds(30));

            if (!browserInit) return false; //si le browser a pas chargé on annule

            browser.LoadUrlWithPostData("https://haapi.ankama.com/json/Ankama/v2/Api/CreateApiKey", bytes); //On envoie la trame
            //Quand elle est finit on traite le résultat dans une autre fonction (FrameLoadEnd)

            int httpCode = 0; //on récupère l'httpcode à titre informatif quand on va afficher l'erreur
            browser.FrameLoadEnd += delegate (object sender, FrameLoadEndEventArgs e) 
            {
                httpCode = returnApiKey(RuntimeHelpers.GetObjectValue(sender), e);
            };

            // tant que apikey a pas changé on attend
            bool boolGetApiKey = System.Threading.SpinWait.SpinUntil(() => (_apiKey != ""), TimeSpan.FromSeconds(30)); 

            if (boolGetApiKey == false || _apiKey == "failed") //si au bout de 30 secondes l'apikey a pas de changement on annule / ou erreur
            {
                Logger.LogError("", LanguageManager.Translate("32", httpCode));
                if (browser != null)
                {
                    if (!browser.IsDisposed)
                    {
                        browser.Dispose();
                    }
                }
                _apiKey = "";
                return false;
            }

            if (browser != null)
            {
                if (!browser.IsDisposed)
                {
                    browser.Dispose();
   
                }
            }
            Console.WriteLine("[2/3] - Retrieving account token");

            try
            {
                // HttpClient creation (with proxy if available)
                var httpClient = !AccountConfig.Proxy.IsValid ?
                                 new HttpClient() :
                                 new HttpClient(new HttpClientHandler
                                 {
                                     Proxy = new WebProxy(AccountConfig.Proxy.Url, false)
                                     {
                                         UseDefaultCredentials = false,
                                         Credentials = new NetworkCredential(AccountConfig.Proxy.Username, AccountConfig.Proxy.Password)
                                     },
                                     PreAuthenticate = true,
                                     UseDefaultCredentials = false
                                 });

                using (httpClient)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("apiKey", _apiKey);
                    HttpResponseMessage response = await httpClient.GetAsync("https://haapi.ankama.com/json/Ankama/v2/Account/CreateToken?game=18");

                    if (response.IsSuccessStatusCode)
                    {
                        Token = (await response.Content.ReadAsJsonAsync()).Value<string>("token");
                        Console.WriteLine("[3/3] - Authenticated");
                        httpClient.Dispose();
                        response.Dispose();
                        _apiKey = "";
                        return true;
                    }

                    httpClient.Dispose();
                    response.Dispose();
                    Logger.LogError("", LanguageManager.Translate("32", response.StatusCode));
                }
                _apiKey = "";
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur {0}", ex);
            }

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
                Logger.LogDebug("reCaptcha", "Getting response..");
                string response = RecaptchaHandler.GetResponse(sitekey);
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
                    if (!HasGroup && _wasScriptRunning)
                    {
                        await Task.Delay(2000);
                        Logger.LogDebug(LanguageManager.Translate("71"), LanguageManager.Translate("76"));
                        Scripts.StartScript();

                        // Only set reset _wasScriptRunning if the script was actually started
                        // Because if the bot received another recaptcha, StartScript will just return because IsBusy will be True
                        if (Scripts.Enabled)
                            _wasScriptRunning = false;
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
            State = AccountStates.DISCONNECTED;
            Logger.LogWarning("Network", LanguageManager.Translate("31"));
            
            if (browser != null)
            {
                if (!browser.IsDisposed)
                {
                    browser.Dispose();
                }
            }
            // In case there was a script enabled
            if (Network.Phase != NetworkPhases.SWITCHING_TO_GAME)
            {
                BubbleBotMain.Instance.Server.SendMessage(new BotInformationsMessage(
                    AccountConfig.Username,
                    Game.Character.Level,
                    (byte)Game.Character.Stats.EnergyPercent,
                    (byte)Game.Character.Inventory.WeightPercent,
                    Game.Character.Inventory.Kamas,
                    Game.Map.Id,
                    Game.Map.CurrentPosition,
                    State.ToFriendlyString()
                ));

                _wasScriptEnabled = Scripts.Enabled;
                Scripts.StopScript();
                Extensions.Flood.Stop();
                // In case the disconnection isnt intentional
                if(!IsIntentionalDisconnection)
                {
                    if (GlobalConfiguration.Instance.AutomaticReconnection)
                    {
                        Task.Run(() =>
                        {
                            Logger.LogMessage(LanguageManager.Translate("12"), LanguageManager.Translate("616", 20));
                            Connect();
                            SpinWait.SpinUntil(() => (Network.Phase == NetworkPhases.GAME), TimeSpan.FromSeconds(20));
                            if (Network.Phase == NetworkPhases.GAME)
                            {
                                if(IsFighting())
                                {
                                    Logger.LogMessage(LanguageManager.Translate("12"), LanguageManager.Translate("625", 180));
                                    SpinWait.SpinUntil(() => (IsFighting() != true), TimeSpan.FromSeconds(180));
                                    if (HasGroup && IsGroupChief)
                                        Group.Chief.Scripts.StartScript();
                                    else if(!HasGroup)
                                        Scripts.StartScript();
                                }
                                if (HasGroup && IsGroupChief)
                                {
                                    SpinWait.SpinUntil(() => (!IsBusy), TimeSpan.FromSeconds(10));
                                    Task.Delay(1500);
                                    Group.Chief.Scripts.StartScript();
                                }
                                else if (!HasGroup)
                                {
                                    SpinWait.SpinUntil(() => (!IsBusy), TimeSpan.FromSeconds(10));
                                    Task.Delay(1500);
                                    Scripts.StartScript();
                                }
                            }
                            else
                            {
                                Logger.LogMessage(LanguageManager.Translate("12"), LanguageManager.Translate("630"));
                                IsIntentionalDisconnection = false;
                                Network.Disconnect("CLIENT_CLOSING");
                            }
                        });
                    }
                }
            }
            IsIntentionalDisconnection = false;
        }


        private async void Planification_Callback(object state)
        {
            if (!AccountConfig.PlanificationActivated)
                return;

            int hour = DateTime.Now.Hour;

            // If the bot is connected and the hour is red
            if (Network.Connected && AccountConfig.Planification[hour] == false && State != AccountStates.FIGHTING)
            {
                Logger.LogInfo("Planificateur", LanguageManager.Translate("584"));
                await Network.Disconnect("CLIENT_CLOSING");
            }
            // If the bot is disconnected and the hour is green
            else if (State == AccountStates.DISCONNECTED && AccountConfig.Planification[hour])
            {
                Logger.LogInfo("Planificateur", LanguageManager.Translate("585"));
                try
                {
                    await Connect();
                }
                catch (Exception ex)
                {
                    Logger?.LogError("", ex.ToString());
                }
            }
        }

        private async void Map_MapLoaded()
        {
            if (!AccountConfig.PlanificationActivated || !_wasScriptEnabled)
                return;

            await Task.Delay(1500);
            Logger.LogInfo("Planificateur", LanguageManager.Translate("583"));
            Scripts.StartScript();
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
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
                    if (browser != null)
                        if (!browser.IsDisposed)
                            browser.Dispose();
                }

                _state = AccountStates.NONE;
                _apiKey = "";
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
                IsIntentionalDisconnection = false;
                _disposedValue = true;
            }
        }

        ~Account() => Dispose(false);

        public void Dispose() => Dispose(true);

        #endregion

    }
}