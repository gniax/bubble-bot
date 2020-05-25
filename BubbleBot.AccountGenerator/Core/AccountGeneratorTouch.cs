using CefSharp;
using CefSharp.OffScreen;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AccountGenerator.Core
{
    class AccountGeneratorTouch
    {
        public ChromiumWebBrowser browser;

#pragma warning disable CS0067 // The event 'AccountGeneratorTouch.RecaptchaReceived' is never used
        public event Action<AccountGeneratorTouch> RecaptchaReceived;
#pragma warning restore CS0067 // The event 'AccountGeneratorTouch.RecaptchaReceived' is never used
        public event Action<AccountGeneratorTouch, bool> RecaptchaResolved;
        //Progarm property
        private int mLongueurName = 10; // 6-19
#pragma warning disable CS0414 // The field 'AccountGeneratorTouch.mLongueurPassword' is assigned but its value is never used
        private int mLongueurPassword = 12;
#pragma warning restore CS0414 // The field 'AccountGeneratorTouch.mLongueurPassword' is assigned but its value is never used
        private int mLongueurMailAlias = 10;
        public bool debugmode = false;
        private int nombreAccount = 0;
        public int MAX_TRY_CONNEXION = Program.MAX_TRYBASIC;
        public int MAX_TRY_REQUEST = Program.MAX_TRYBASIC;

        //Random creation
        private static int seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
        private static readonly object lockObj = new object();
        private static Random random2 = new Random();

        //Account1
        private string mUsername1;
        private string mPassword1;
        private string mMail1;
        private string mNickname1;
        private string mProxyAdresse1;
        private string mApikey1;

        //Account2
        private string mUsername2;
        private string mPassword2;
        private string mMail2;
        private string mNickname2;
        private string mProxyAdresse2;
        private string mApikey2;

        //Account3
        private string mUsername3;
        private string mPassword3;
        private string mMail3;
        private string mNickname3;
        private string mProxyAdresse3;
        private string mApikey3;

        //gestion programme 
        private int counterAccount = 1;
        public bool allfinished = false; //Attend la fin de la creation 
        public int nbgoodcreation = 0;

        //liaison classe appelante 
        public string outputAcc1 = "";
        public string outputAcc1p = "";
        public string outputAcc2 = "";
        public string outputAcc2p = "";
        public string outputAcc3 = "";
        public string outputAcc3p = "";
        public string debug = "";


        public AccountGeneratorTouch(string accountname, string accountpassword, string accountmail, string accountproxyadresse, string accountapikey, int nbcompte)
        {
            //On choisi pseudo
            if (accountname != "")
            {
                mUsername1 = accountname;
                mUsername2 = accountname;
                mUsername3 = accountname;
            }
            else
            {
                mUsername1 = GetRandomString(mLongueurName);
                mUsername2 = GetRandomString(mLongueurName);
                mUsername3 = GetRandomString(mLongueurName);
            }

            //On choisi mot de passe
            if (accountpassword != "")
            {
                mPassword1 = accountpassword;
                mPassword2 = accountpassword;
                mPassword3 = accountpassword;
            }
            else
            {
                mPassword1 = GetRandomString(16);
                mPassword2 = GetRandomString(16);
                mPassword3 = GetRandomString(16);
            }

            //On choisi mail
            if (accountmail != "")
            {
                mMail1 = accountmail + "%2B" + GetRandomStringMini(mLongueurMailAlias) + "%40gmail.com";
                mMail2 = accountmail + "%2B" + GetRandomStringMini(mLongueurMailAlias) + "%40gmail.com";
                mMail3 = accountmail + "%2B" + GetRandomStringMini(mLongueurMailAlias) + "%40gmail.com";
            }

            //On choisi adresse du proxy
            if (accountproxyadresse != "")
            {
                mProxyAdresse1 = accountproxyadresse;
                mProxyAdresse2 = accountproxyadresse;
                mProxyAdresse3 = accountproxyadresse;
            }
            //On set captcha key
            if (accountapikey != "")
            {
                mApikey1 = accountapikey;
                mApikey2 = accountapikey;
                mApikey3 = accountapikey;
            }

            //Nickname pour la version touch
            mNickname1 = GetRandomString(mLongueurName);
            mNickname2 = GetRandomString(mLongueurName);
            mNickname3 = GetRandomString(mLongueurName);

            nombreAccount = nbcompte;
            //Pour les infos random autre
            /* mDayDate1 = GetRandomInt(1, 30);
             mDayDate2 = GetRandomInt(1, 30);
             mDayDate3 = GetRandomInt(1, 30);
             mMonthDate1 = GetRandomInt(1, 12);
             mMonthDate2 = GetRandomInt(1, 12);
             mMonthDate3 = GetRandomInt(1, 12);
             mYearDate1 = GetRandomInt(1990, 2000);
             mYearDate2 = GetRandomInt(1990, 2000);
             mYearDate3 = GetRandomInt(1990, 2000);
             */
        }



#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task outDebugSafe(string dbgtxt)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            debug = debug + "\n" + dbgtxt;
        }

        async private Task SetProxy(ChromiumWebBrowser cwb, string Address)
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

        public async void CreationCompteStart()
        {
            Console.WriteLine("Information sur la création du compte 1 :");
            Console.WriteLine("Pseudo:{0}   Password:{1}   Mail:{2}  Proxy:{3}", mUsername1, mPassword1, mMail1, mProxyAdresse1);
            Console.WriteLine("Information sur la création du compte 2 :");
            Console.WriteLine("Pseudo:{0}   Password:{1}   Mail:{2}  Proxy:{3}", mUsername2, mPassword2, mMail2, mProxyAdresse2);
            Console.WriteLine("Information sur la création du compte 3 :");
            Console.WriteLine("Pseudo:{0}   Password:{1}   Mail:{2}  Proxy:{3}", mUsername3, mPassword3, mMail3, mProxyAdresse3);

            //Chargement chromium
            bool browserInit;
            browser = new ChromiumWebBrowser("about:blank"); //about:blank
            browserInit = System.Threading.SpinWait.SpinUntil(() => (browser.IsBrowserInitialized), TimeSpan.FromSeconds(60));
            if (!browserInit)
            {
                await outDebugSafe("Erreur chargement WebBrowser !");//si le browser a pas chargé on annule
            }
            else
            {
                await outDebugSafe("Browser open !");
            }


            await SetProxy(browser, "http://" + mProxyAdresse1);


            browser.Load("https://proxyconnection.touch.dofus.com/haapi/getForumPostsList?lang=fr&topicId=24993");//https://proxyconnection.touch.dofus.com/haapi/getForumPostsList?lang=fr&topicId=24993

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            DofusConnection();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.


        }


        private async Task DofusConnection()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Connexion au site DofusTouch...    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
            bool chargement = false;
            int nbtryload = MAX_TRY_CONNEXION;
            string tmploadres = "";
            JavascriptResponse takeInfo21;
            while (chargement == false && nbtryload > 0)
            {
                while (browser.IsLoading)
                {
                    Thread.Sleep(200);
                }
                takeInfo21 = await browser.GetMainFrame().EvaluateScriptAsync("document.querySelector('body > pre').innerText;");
                tmploadres = JsonConvert.SerializeObject(takeInfo21.Result);
                //Console.WriteLine(tmploadres);
                await outDebugSafe(tmploadres);

                if (tmploadres == '\"' + "[]" + '\"')
                {
                    chargement = true;
                }
                else
                {
                    chargement = false;
                    browser.Load("https://proxyconnection.touch.dofus.com/haapi/getForumPostsList?lang=fr&topicId=24993");
                    nbtryload--;
                }
            }

            //Si le chargement ne fonctiuonne pas on quitte
            if (chargement == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Impossible de se connecter au site DofusTouch...    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
                browser.Dispose();
                allfinished = true;
                return;
            }

            Thread.Sleep(2000);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Bypass du captcha...    TOTALACCOUNT:{1}", counterAccount, nombreAccount));

            string captchares = await HandleRecaptcha("6Leicx0TAAAAAE-R05fbh9qqtID2XDtkOBd7-KnF", 3);
            if (captchares == "Error")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Impossible de résoudre le captcha...    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
                browser.Dispose();
                allfinished = true;
                return;
            }

            string ckey = "";
            Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(captchares));
            if (dictionaryRes.ContainsKey("data")) //On récupère la reponse anti-captcha
            {
                ckey = (string)dictionaryRes["data"];

            }

            if (ckey == "")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Impossible de résoudre le captcha...    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
                browser.Dispose();
                allfinished = true;
                return;
            }
            await outDebugSafe(ckey);

            //Thread.Sleep(2000);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Demande de creation de compte...    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
            bool getHeader = false;
            string headerGuest = "";
            int nbtryheader = MAX_TRY_REQUEST;
            while (getHeader == false && nbtryheader > 0)
            {
                await browser.GetMainFrame().EvaluateScriptAsync("var request = new XMLHttpRequest();");
                //Initialisation des valeurs de la requête 

                await browser.GetMainFrame().EvaluateScriptAsync("request.open('GET'," + '\'' + "https://haapi.ankama.com/json/Ankama/v2/Account/CreateGuest?game=18&lang=fr&web_params%5B%5D=&captcha_token=" + ckey + '\'' + ", false);");
                Thread.Sleep(500);
                //Envoie de la requête 
                await browser.GetMainFrame().EvaluateScriptAsync("request.send();");
                Thread.Sleep(4000);
                //Recuperation du header de la reponse
                JavascriptResponse takeInfo85 = await browser.GetMainFrame().EvaluateScriptAsync("request.getAllResponseHeaders();");
                headerGuest = JsonConvert.SerializeObject(takeInfo85.Result);
                // Console.WriteLine(headerGuest);
                await outDebugSafe(headerGuest);
                
                //"x-password: ttvA89QbSG79\r\ncontent-type: application/json\r\nx-duration: 81.073999\r\n"
                if (headerGuest.Contains("x-password"))
                {
                    await outDebugSafe("Le header a bien ete recue !");
                    //Console.WriteLine("Le header a bien ete recue !");
                    getHeader = true;
                }
                else
                {
                    await outDebugSafe("X-password non trouver !");
                    //Console.WriteLine("X-password non trouver !");
                    getHeader = false;
                    nbtryheader--;
                }

            }
            //ankama_captcha_incorrect
            if (getHeader == false)
            {
                JavascriptResponse takeInfo85250 = await browser.GetMainFrame().EvaluateScriptAsync("request.response;");
                await outDebugSafe(JsonConvert.SerializeObject(takeInfo85250.Result));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Echec de la requête de demande de creation de compte...    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
                browser.Dispose();
                allfinished = true;
                return;
            }

            string pLogin = "";
            string pPassword = "";
            string pMail = "";
            string pNickname = "";
            string pBirth = "";
            string guestId = "";
            string guestPass = "";

            if (getHeader == true)
            {
                JavascriptResponse takeInfo90 = await browser.GetMainFrame().EvaluateScriptAsync("request.response;");
                string dataGuest = JsonConvert.SerializeObject(takeInfo90.Result);
                //Console.WriteLine(dataGuest);
                await outDebugSafe(dataGuest);
                int startIndex = headerGuest.IndexOf("x-password: ");
                int endIndex = headerGuest.IndexOf(@"\r\", startIndex);
                string amountString = headerGuest.Substring(startIndex, endIndex - startIndex);
                //Console.WriteLine(amountString);
                await outDebugSafe(amountString);

                //Recuperation de la reponse
                Thread.Sleep(2000);

                // Console.WriteLine(dataGuest.Substring(posId + 7, 18));
                int Index1 = dataGuest.IndexOf("[GUEST]");
                int Index2 = dataGuest.IndexOf(@"\", Index1);
                string guestString = dataGuest.Substring(Index1, Index2 - Index1);
                //Console.WriteLine(guestString);
                await outDebugSafe(guestString);
                guestId = guestString.Substring(7, guestString.Length - 7);
                guestPass = amountString.Substring(12, amountString.Length - 12);
                await outDebugSafe(string.Format("INFO GUEST:  id:{0}    password:{1}", guestId, guestPass));
                //Console.WriteLine(string.Format("INFO GUEST:  id:{0}    password:{1}", guestId, guestPass));
            }

            Thread.Sleep(2000);

            if (counterAccount == 1)
            {
                pLogin = mUsername1;
                pPassword = mPassword1;
                pMail = mMail1;
                pNickname = mNickname1;
                pBirth = "920246400000";
            }
            else if (counterAccount == 2)
            {
                pLogin = mUsername2;
                pPassword = mPassword2;
                pMail = mMail2;
                pNickname = mNickname2;
                pBirth = "920246400000";

            }
            else if (counterAccount == 3)
            {
                pLogin = mUsername3;
                pPassword = mPassword3;
                pMail = mMail3;
                pNickname = mNickname3;
                pBirth = "920246400000";
            }
            else
            {
                await outDebugSafe("Erreur, les comptes on déjà été créer...");
                //Console.WriteLine("Erreur, les comptes on déjà été créer...");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Validation de la creation du compte...    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
            bool postData = false;
            int nbtrypostdata = MAX_TRY_REQUEST;

            while (postData == false && nbtrypostdata > 0)
            {
                string postAccount = string.Format(@"https://proxyconnection.touch.dofus.com/haapi/validateGuest?login={0}&password={1}&email={2}&nickname={3}&birthDateTimestamp={4}&parentEmail=&guestLogin=%5BGUEST%5D{5}&guestPassword={6}&lang=fr", pLogin, pPassword, pMail, pNickname, pBirth, guestId, guestPass);
                //Console.WriteLine(postAccount);
                await outDebugSafe(postAccount);

                await browser.GetMainFrame().EvaluateScriptAsync("request.open('GET'," + '\'' + postAccount + '\'' + ", false);");
                Thread.Sleep(500);
                await browser.GetMainFrame().EvaluateScriptAsync("request.send();");
                Thread.Sleep(4000);
                JavascriptResponse takeInfo100 = await browser.GetMainFrame().EvaluateScriptAsync("request.response;");
                string resultCreate = JsonConvert.SerializeObject(takeInfo100.Result);
                // Console.WriteLine(resultCreate);
                await outDebugSafe(resultCreate);

                if (resultCreate.Contains("duration"))
                {
                    await outDebugSafe("Le compte a été créer avec succès !");
                    //Console.WriteLine("Le compte a été créer avec succès !");
                    postData = true;
                }
                else
                {
                    await outDebugSafe("Erreur a la creation du compte !");
                    // Console.WriteLine("Erreur a la creation du compte !");
                    nbtrypostdata--;
                }
            }

            //si creation impossible on passe a la suite
            if (postData == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Echec de la creation du compte...    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
                browser.Dispose();
                allfinished = true;
                return;
            }

            //On doit traiter les erreurs de creation
            if (counterAccount == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Compte créer avec succès !    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
                outputAcc1 = mUsername1;
                outputAcc1p = mPassword1;
                counterAccount++;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                DofusConnection();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.

            }
            else if (counterAccount == 2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Compte créer avec succès !    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
                outputAcc2 = mUsername2;
                outputAcc2p = mPassword2;
                counterAccount++;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                DofusConnection();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            }
            else if (counterAccount == 3)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format("ACCOUNT[{0}/3]:Compte créer avec succès !    TOTALACCOUNT:{1}", counterAccount, nombreAccount));
                outputAcc3 = mUsername3;
                outputAcc3p = mPassword3;
                counterAccount++;
                browser.Dispose();
                allfinished = true;
            }
            else
            {
                await outDebugSafe("Erreur, les comptes on déjà été créer...");
                //Console.WriteLine("Erreur, les comptes on déjà été créer...");
            }

        }

        public async Task StartCaptchaBypass()
        {
            string mSitekey = "";
            Thread.Sleep(500);
            Console.WriteLine("Captcha reçue.");
            Console.WriteLine("On passe au traitement du captcha...");

            bool success = false;
            while (success == false)
            {
                JavascriptResponse takeInfo2 = await browser.EvaluateScriptAsync("document.querySelector('#challenge-form > script').dataset;");
                string rawess = JsonConvert.SerializeObject(takeInfo2.Result);
                Console.WriteLine(rawess);
                Dictionary<string, object> dictionaryResds = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(rawess));

                if (dictionaryResds.ContainsKey("sitekey")) //On récupère l'apikey
                {
                    string monray = (string)dictionaryResds["sitekey"];
                    Console.WriteLine(monray);
                    mSitekey = monray;
                    success = true;
                }
                else
                {
                    Console.WriteLine("Sitekey non trouver !");
                    Thread.Sleep(2000);
                    success = false;
                }
            }

            string captchares;
            captchares = await HandleRecaptcha(mSitekey, 1);

            string ckey = "";
            Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(captchares));
            if (dictionaryRes.ContainsKey("data")) //On récupère la reponse anti-captcha
            {
                ckey = (string)dictionaryRes["data"];

            }
            Thread.Sleep(500);

            Console.WriteLine(ckey);

            await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').style.display =  " + '\'' + "block" + '\'' + ";");
            Thread.Sleep(1000);
            await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').innerHTML = " + '\'' + ckey + '\'' + ";");
            Thread.Sleep(1000);
            await MakeSnapshot(0);
            Thread.Sleep(5000);
            await browser.EvaluateScriptAsync(@"document.querySelector('#challenge-form input[type=submit]').click();");
            //Thread.Sleep(5000);
            //await MakeSnapshot(1);
            Console.WriteLine("Requête terminer Captcha !");
            Thread.Sleep(5000);
            await MakeSnapshot(6);

            Thread.Sleep(5000);
            Console.WriteLine("Normalement on a passez le captcha !");
            // captchabypassed = true;
        }

        public static string GetRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random2.Next(s.Length)]).ToArray());
        }
        public static string GetRandomStringMini(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random2.Next(s.Length)]).ToArray());
        }

        public static int GetRandomInt(int min, int max)
        {
            lock (lockObj)
            {
                return min <= max ? random.Value.Next(min, max) : random.Value.Next(max, min);
            }
        }




        public async Task<String> HandleRecaptcha(string sitekey, int tries = 3)
        {

            // If _wasScriptRunning was already true, don't change it        
            // RecaptchaReceived?.Invoke(this);

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                //Console.WriteLine("reCaptcha Getting response..");
                await outDebugSafe("reCaptcha Getting response..");
                string response = "";
                RecaptchaHandler cpttask = new RecaptchaHandler();

                response = await cpttask.GetResponse(sitekey);
                //Console.WriteLine("reCaptcha Got response.");
                await outDebugSafe("reCaptcha Got response.");
                // If the response is null, its because the user didn't enter an anti-captcha key
                if (response == null)
                {
                    // We shouldn't leave this True
                    // Console.WriteLine("Erreur Anti-captcha personnal key !");
                    await outDebugSafe("Erreur Anti-captcha personnal key !");
                    RecaptchaResolved?.Invoke(this, false);
                }
                else
                {

                    //Console.Write(sw.Elapsed.TotalSeconds);
                    await outDebugSafe("Réponce reçue en " + sw.Elapsed.TotalSeconds + " secondes.");


                    dynamic msg = new ExpandoObject();
                    msg.call = "recaptchaResponse";
                    msg.data = response;
                    string raw = JsonConvert.SerializeObject(msg);

                    /* Console.Write("Clée reçue =");
                     Console.Write(raw);
                     Console.WriteLine(" ");
                     */
                    await outDebugSafe("Clée reçue =" + raw);

                    return raw;
                    // await Network.SendRawAsync(raw);

                    /*
                    if (State == AccountStates.RECAPTCHA)
                    {
                        State = AccountStates.NONE;
                    }
                    */
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                //  Console.Write("Erreur reCaptcha");
                //Console.Write(ex.Message);
                if (tries < 3)
                {
                    // Console.Write("Captcha non résolut tentative numéro ");
                    //   Console.Write(++tries);
                    tries++;
                    // Console.WriteLine(" ");
                    await HandleRecaptcha(sitekey, tries);
                }

            }
            return "Error";
        }


        private async Task MakeSnapshot(int number)
        {
            Thread.Sleep(500);
            var task = browser.ScreenshotAsync();
            await task.ContinueWith(x =>
            {
                var screenshotPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DebugJackouille" + number + ".png");
                task.Result.Save(screenshotPath);
                task.Result.Dispose();
            }, TaskScheduler.Default);
            Console.WriteLine("Fin screen");

        }



    }
}
