using CefSharp;
using CefSharp.OffScreen;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace AccountGenerator.Core
{
    class AccountCreation
    {
        public ChromiumWebBrowser browser;
        //Progarm property
        private int mLongueurName = 16; // 6-19
        private int mLongueurPassword = 12;
        private int mLongueurMailAlias = 10;


        //Account1
        private bool mAccountCreated1 = false;
        private string mAccountError1;
        private string mUsername1;
        private string mPassword1;
        private string mMail1;
        private string mProxyAdresse1;
        private int mProxyPort1;
        private string mApikey1;
        private int mDayDate1;
        private int mMonthDate1;
        private int mYearDate1;
        //Account2
        private bool mAccountCreated2 = false;
        private string mAccountError2;
        private string mUsername2;
        private string mPassword2;
        private string mMail2;
        private string mProxyAdresse2;
        private int mProxyPort2;
        private string mApikey2;
        private int mDayDate2;
        private int mMonthDate2;
        private int mYearDate2;
        //Account3
        private bool mAccountCreated3 = false;
        private string mAccountError3;
        private string mUsername3;
        private string mPassword3;
        private string mMail3;
        private string mProxyAdresse3;
        private int mProxyPort3;
        private string mApikey3;
        private int mDayDate3;
        private int mMonthDate3;
        private int mYearDate3;
        //Variable global de traitement
        private string captcharesult = "";
        private int selectedAccount = 1;
        private string mSitekey = "";
        private bool mBusy = false;
        private int mCapctha = 0;
        private int mCaptchamode = 0; //0-Primaire(Page specifique au captcha) 1-Secondaire(Page de l'inscription)

        //Random creation
        private static int seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
        private static readonly object lockObj = new object();
        private static Random random2 = new Random();


        private string mCkey = "";

        private string captchamethode = "";
        private bool captchabypassed = false;
        private bool captchawork = false;
        public event Action<AccountCreation> RecaptchaReceived;
        public event Action<AccountCreation, bool> RecaptchaResolved;

        public AccountCreation(string accountname, string accountpassword, string accountmail, string accountproxyadresse, int accountproxyport, string accountapikey)
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
                mMail1 = accountmail + "+" + GetRandomStringMini(mLongueurMailAlias) + "@gmail.com";
                mMail2 = accountmail + "+" + GetRandomStringMini(mLongueurMailAlias) + "@gmail.com";
                mMail3 = accountmail + "+" + GetRandomStringMini(mLongueurMailAlias) + "@gmail.com";
            }

            //On choisi adresse du proxy
            if (accountproxyadresse != "")
            {
                mProxyAdresse1 = accountproxyadresse;
                mProxyAdresse2 = accountproxyadresse;
                mProxyAdresse3 = accountproxyadresse;
            }

            //On choisi port du proxy
            if (accountproxyport >= 0)
            {
                mProxyPort1 = accountproxyport;
                mProxyPort2 = accountproxyport;
                mProxyPort3 = accountproxyport;
            }
            //On set captcha key
            if (accountproxyport >= 0)
            {
                mApikey1 = accountapikey;
                mApikey2 = accountapikey;
                mApikey3 = accountapikey;
            }
            //Pour les infos random autre
            mDayDate1 = GetRandomInt(1, 30);
            mDayDate2 = GetRandomInt(1, 30);
            mDayDate3 = GetRandomInt(1, 30);
            mMonthDate1 = GetRandomInt(1, 12);
            mMonthDate2 = GetRandomInt(1, 12);
            mMonthDate3 = GetRandomInt(1, 12);
            mYearDate1 = GetRandomInt(1990, 2000);
            mYearDate2 = GetRandomInt(1990, 2000);
            mYearDate3 = GetRandomInt(1990, 2000);
        }

        public async void CreateAccount()
        {
            bool browserInit;
            //On charge le navigateur vide
            browser = new ChromiumWebBrowser("about:blank"); //about:blank

            //browser.LoadingStateChanged += BrowserLoadingStateChanged;

            //tant que le browser est pas initialisé on attend (tiemout 30sec)
            browserInit = System.Threading.SpinWait.SpinUntil(() => (browser.IsBrowserInitialized), TimeSpan.FromSeconds(60));

            if (!browserInit)
            {
                Console.WriteLine("Erreur chargement WebBrowser !");  //si le browser a pas chargé on annule
            }
            else
            {
                Console.WriteLine("Browser open !");
            }
            browser.FrameLoadEnd += BrowserAccountCreation;
            /*  browser.FrameLoadEnd += delegate (object sender, FrameLoadEndEventArgs e)
              {
                  BrowserAccountCreation(RuntimeHelpers.GetObjectValue(sender), e);
              };
              */
            Console.WriteLine("Information sur la création du compte 1 :");
            Console.WriteLine("Pseudo:{0}   Password:{1}   Mail:{2}  Proxy:{3}:{4}   ApiKey:{5}   Jour:{6}   Mois:{7}   Année:{8}", mUsername1, mPassword1, mMail1, mProxyAdresse1, mProxyPort1, mApikey1, mDayDate1, mMonthDate1, mYearDate1);
            Console.WriteLine("Information sur la création du compte 2 :");
            Console.WriteLine("Pseudo:{0}   Password:{1}   Mail:{2}  Proxy:{3}:{4}   ApiKey:{5}   Jour:{6}   Mois:{7}   Année:{8}", mUsername2, mPassword2, mMail2, mProxyAdresse2, mProxyPort2, mApikey2, mDayDate2, mMonthDate2, mYearDate2);
            Console.WriteLine("Information sur la création du compte 3 :");
            Console.WriteLine("Pseudo:{0}   Password:{1}   Mail:{2}  Proxy:{3}:{4}   ApiKey:{5}   Jour:{6}   Mois:{7}   Année:{8}", mUsername3, mPassword3, mMail3, mProxyAdresse3, mProxyPort3, mApikey3, mDayDate3, mMonthDate3, mYearDate3);
            //browser.LoadingStateChanged += BrowserLoadingStateChanged;


            // browser.FrameLoadEnd += BrowserAccountCreation;
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
            //browser.Load("https://www.dofus.com/fr/mmorpg/jouer");
            browser.GetBrowser().MainFrame.LoadUrl("https://www.dofus.com/fr/mmorpg/jouer");
            while (mAccountCreated1 == false)
            {
                Thread.Sleep(2000);
            }
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
            captcharesult = "";
            mSitekey = "";
            mCapctha = 0;
            mCaptchamode = 0;
            // browser.Load("https://www.dofus.com/fr/mmorpg/jouer");
            browser.GetBrowser().MainFrame.LoadUrl("https://www.dofus.com/fr/mmorpg/jouer");
            while (mAccountCreated2 == false)
            {
                Thread.Sleep(2000);
            }
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
            captcharesult = "";
            mSitekey = "";
            mCapctha = 0;
            mCaptchamode = 0;
            browser.GetBrowser().MainFrame.LoadUrl("https://www.dofus.com/fr/mmorpg/jouer");
            //browser.Load("https://www.dofus.com/fr/mmorpg/jouer");
            while (mAccountCreated3 == false)
            {
                Thread.Sleep(2000);
            }
            Cef.GetGlobalCookieManager().DeleteCookies("", "");



        }
        public async void BrowserAccountCreation(object sender, FrameLoadEndEventArgs e)
        {
            Console.WriteLine("Wow");
            Console.WriteLine(e.Frame.Url);

            if (e.Frame.Url.Contains("recaptcha/api/fallback") && mCapctha == 0)
            {
                mCapctha = 1;
                //StartCaptchaBypass();
            }

            if (e.Frame.Url.Contains("recaptcha/api2/anchor") && mCapctha == 0)
            {
                mCapctha = 1;
                StartCaptchaBypass();
            }

            if (e.Frame.Url.Contains("www.dofus.com/fr/mmorpg/jouer?__cf_chl_captcha_tk__="))
            {
                //await VerifBypassSecondCaptcha();
                CreateAccountMode2();

            }
            if (e.Frame.Url == "https://www.dofus.com/fr/mmorpg/jouer")
            {
                Thread.Sleep(2000);
                await MakeSnapshot(0);
            }
        }

        public async Task StartCaptchaBypass()
        {
            Thread.Sleep(500);
            Console.WriteLine("Captcha reçue.");
            Console.WriteLine("On passe au traitement du captcha...");
            await MakeSnapshot(1);
            Thread.Sleep(6000);
            JavascriptResponse takeMode = await browser.GetMainFrame().EvaluateScriptAsync(@"document.querySelector('#ak_field_4').value;");
            string takedmode = JsonConvert.SerializeObject(takeMode.Result);
            if (takedmode == '\"' + "Terminer l'inscription" + '\"')
            {
                Console.WriteLine(takedmode);
                mCaptchamode = 1;
                Console.WriteLine("On est en mode captcha secondaire !");
            }
            else
            {
                Console.WriteLine(takedmode);
                Console.WriteLine("On est en mode captcha primaire !");
                mCaptchamode = 2;
            }

            //On recupère la sitekey à envoyer à anti-captcha
            if (mCaptchamode == 2)
            {
                bool success = false;
                while (success == false)
                {
                    JavascriptResponse takeInfo2 = await browser.EvaluateScriptAsync("document.querySelector('#challenge-form > script').dataset;", TimeSpan.FromSeconds(5));
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
                mCapctha = 2;
            }
            else if (mCaptchamode == 1)
            {
                /*  string captchares;
                  captchares = await HandleRecaptcha("6LfbFRsUAAAAACrqF5w4oOiGVxOsjSUjIHHvglJx", 1);

                  string ckey = "";
                  Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(captchares));
                  if (dictionaryRes.ContainsKey("data")) //On récupère la reponse anti-captcha
                  {
                      ckey = (string)dictionaryRes["data"];
                      captcharesult = ckey;
                  }
                  Thread.Sleep(500);
                  //await browser.EvaluateScriptAsync(@"document.querySelector('#recaptcha-token').value =  " + '\'' + ckey + '\'' + ";");
                  // await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').style.display =  " + '\'' + "block" + '\'' + ";");
                  //Thread.Sleep(1000);
                  //await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').innerHTML = " + '\'' + captcharesult + '\'' + ";");
                  Thread.Sleep(1000);
                  */
                //Console.WriteLine("Captcha injecté mais pas soumis au formulaire.Attente de l'injection des informations de compte.");
                mCapctha = 2;
                await CreateAccountMode1();
            }
            else
            {
                Console.WriteLine("Erreur traitement du captcha !");
            }
            // captchabypassed = true;
        }

        public async Task CreateAccountMode1()
        {
            Console.WriteLine("Attente de bypass le captcha...");
            Thread.Sleep(1000);
            Console.WriteLine("Attent chargement de la page Web...");
            while (browser.IsLoading)
            {
                Thread.Sleep(500);
            }
            /* Console.WriteLine("Focused frame...");
             await browser.GetFocusedFrame().EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response-1').innerHTML = " + '\'' + captcharesult + '\'' + ";");
             Console.WriteLine("Main frame...");
             await browser.GetMainFrame().EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response-1').innerHTML = " + '\'' + captcharesult + '\'' + ";");
             // await browser.EvaluateScriptAsync(@"document.querySelector('#recaptcha-token').value =  " + '\'' + captcharesult + '\'' + ";");
             */
            //      if (mCaptchamode == 1 && mCapctha == 2)
            //    {
            await MakeSnapshot(1);
            Thread.Sleep(2000);
            browser.SetZoomLevel(-2);
            Thread.Sleep(2000);
            //Def des variable attribuant les info de compte

            /*    Console.WriteLine("TESTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT");
                JavascriptResponse testCapt = await browser.GetMainFrame().EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response-1').innerText;");
                string testCaptss = JsonConvert.SerializeObject(testCapt.Result);
                if (testCaptss == '\"'.ToString() + '\"'.ToString())
                {
                    Console.WriteLine("Valeur key : ");
                    Console.WriteLine(testCaptss);
                    Console.WriteLine("Clée non trouver !");
                    Thread.Sleep(1000);

                }
                else
                {
                    Console.WriteLine("Valeur key : ");
                    Console.WriteLine(testCaptss);
                    Console.WriteLine("Clée trouver !");
                    Thread.Sleep(1000);

                }

                */
            Thread.Sleep(2000);
            await BypassCaptchaInscription();
            Thread.Sleep(2000);
            /*    Console.WriteLine("TESTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT2");
                JavascriptResponse testCaptz = await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response-1').innerText;");
                string testCaptzss = JsonConvert.SerializeObject(testCaptz.Result);
                if (testCaptzss == '\"'.ToString() + '\"'.ToString())
                {
                    Console.WriteLine("Valeur key : ");
                    Console.WriteLine(testCaptzss);
                    Console.WriteLine("Clée non trouver !");
                    Thread.Sleep(1000);

                }
                else
                {
                    Console.WriteLine("Valeur key : ");
                    Console.WriteLine(testCaptzss);
                    Console.WriteLine("Clée trouver !");
                    Thread.Sleep(1000);

                }
                */
            await InjecteAccountInformation();
            Thread.Sleep(2000);

            while (true)
            {
                Thread.Sleep(200);
                JavascriptResponse takeVerife = await browser.GetMainFrame().EvaluateScriptAsync(@"document.querySelector('body > div.ak-mobile-menu-scroller > div > div > div:nth-child(1) > div > div > div > div.ak-inner-block > div > div.col-md-8 > div > div > div > div.ak-title').innerText;");
                string takedVerife = JsonConvert.SerializeObject(takeVerife.Result);
                if (takedVerife == '\"' + "Confirmez votre inscription" + '\"')
                {
                    Console.WriteLine("Le compte a bien été créer !");
                    browser.GetBrowser().MainFrame.LoadUrl("https://account.ankama.com/sso?action=logout&from=https%3A%2F%2Fwww.dofus.com%2Ffr");
                    // browser.Load("https://account.ankama.com/sso?action=logout&from=https%3A%2F%2Fwww.dofus.com%2Ffr");
                    Console.WriteLine("Déconnexion du compte!");
                    Thread.Sleep(10000);
                    await MakeSnapshot(2);
                    if (selectedAccount == 1)
                    {
                        mAccountCreated1 = true;
                        selectedAccount++;
                        Console.WriteLine("On passe au compte " + selectedAccount.ToString() + ".");
                    }
                    else if (selectedAccount == 2)
                    {
                        mAccountCreated2 = true;
                        selectedAccount++;
                        Console.WriteLine("On passe au compte " + selectedAccount.ToString() + ".");
                    }
                    else if (selectedAccount == 3)
                    {
                        mAccountCreated3 = true;
                        selectedAccount++;
                        Console.WriteLine("On passe au compte " + selectedAccount.ToString() + ".");
                    }
                    else if (selectedAccount == 4)
                    {
                        Console.WriteLine("On à fini la création des comptes !");
                    }
                    Thread.Sleep(2000);

                    //captcharesult = "";
                    // mSitekey = "";
                    //mCapctha = 0;
                    //mCaptchamode = 0;
                    //browser.GetMainFrame().Delete();

                    //browser.Load("https://www.dofus.com/fr/mmorpg/jouer");
                    //await CreateAccountMode1();
                    break;
                }
                else
                {
                    Console.WriteLine("Erreur a la création du compte !");
                    //break;
                }
                JavascriptResponse verifValidation = await browser.GetMainFrame().EvaluateScriptAsync(@"document.querySelector('#ak_field_4').value;");
                string takeValide = JsonConvert.SerializeObject(verifValidation.Result);
                if (takeValide == '\"' + "Terminer l'inscription" + '\"')
                {
                    //  int nbtryingvalidation = 0;
                    //    while (nbtryingvalidation < 5)
                    //  {
                    Console.WriteLine(takeValide);
                    Console.WriteLine("On est sur la page d'inscription, donc on réinjecte les infos !");
                    await InjecteAccountInformation();
                    //   nbtryingvalidation++;
                    // }
                }
                else
                {
                    Console.WriteLine(takeValide);
                    Console.WriteLine("On est pas sur la page d'inscription !");
                }
            }
            //}
        }

        public async Task CreateAccountMode2()
        {
            browser.SetZoomLevel(-2);
            /*Console.WriteLine("TESTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT");
            JavascriptResponse testCapt = await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response-1').innerText;");
            string testCaptss = JsonConvert.SerializeObject(testCapt.Result);
            if (testCaptss == '\"'.ToString() + '\"'.ToString())
            {
                Console.WriteLine("Valeur key : ");
                Console.WriteLine(testCaptss);
                Console.WriteLine("Clée non trouver !");
                Thread.Sleep(1000);

            }
            else
            {
                Console.WriteLine("Valeur key : ");
                Console.WriteLine(testCaptss);
                Console.WriteLine("Clée trouver !");
                Thread.Sleep(1000);

            }
            */
            Thread.Sleep(2000);
            await BypassCaptchaInscription();
            Thread.Sleep(2000);
            /* Console.WriteLine("TESTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT2");
             JavascriptResponse testCaptz = await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response-1').innerText;");
             string testCaptzss = JsonConvert.SerializeObject(testCaptz.Result);
             if (testCaptzss == '\"'.ToString() + '\"'.ToString())
             {
                 Console.WriteLine("Valeur key : ");
                 Console.WriteLine(testCaptzss);
                 Console.WriteLine("Clée non trouver !");
                 Thread.Sleep(1000);

             }
             else
             {
                 Console.WriteLine("Valeur key : ");
                 Console.WriteLine(testCaptzss);
                 Console.WriteLine("Clée trouver !");
                 Thread.Sleep(1000);

             }
             */
            await InjecteAccountInformation();

            while (true)
            {
                Thread.Sleep(200);
                while (browser.IsLoading)
                {
                    Thread.Sleep(500);
                }
                Thread.Sleep(500);
                JavascriptResponse takeVerife = await browser.EvaluateScriptAsync(@"document.querySelector('body > div.ak-mobile-menu-scroller > div > div > div:nth-child(1) > div > div > div > div.ak-inner-block > div > div.col-md-8 > div > div > div > div.ak-title').innerText;");
                string takedVerife = JsonConvert.SerializeObject(takeVerife.Result);
                if (takedVerife == '\"' + "Confirmez votre inscription" + '\"')
                {
                    Console.WriteLine("Le compte a bien été créer !");
                    browser.GetBrowser().MainFrame.LoadUrl("https://account.ankama.com/sso?action=logout&from=https%3A%2F%2Fwww.dofus.com%2Ffr");
                    //browser.Load("https://account.ankama.com/sso?action=logout&from=https%3A%2F%2Fwww.dofus.com%2Ffr");
                    Console.WriteLine("Déconnexion du compte!");
                    Thread.Sleep(9000);
                    if (selectedAccount == 1)
                    {
                        mAccountCreated1 = true;
                        selectedAccount++;
                        Console.WriteLine("On passe au compte " + selectedAccount.ToString() + ".");
                    }
                    else if (selectedAccount == 2)
                    {
                        mAccountCreated2 = true;
                        selectedAccount++;
                        Console.WriteLine("On passe au compte " + selectedAccount.ToString() + ".");
                    }
                    else if (selectedAccount == 3)
                    {
                        mAccountCreated3 = true;
                        selectedAccount++;
                        Console.WriteLine("On passe au compte " + selectedAccount.ToString() + ".");
                    }
                    if (selectedAccount == 4)
                    {
                        Console.WriteLine("On à fini la création des comptes !");
                    }
                    Thread.Sleep(2000);
                    //captcharesult = "";
                    // mSitekey = "";
                    //mCapctha = 0;
                    //mCaptchamode = 0;
                    // browser.Load("https://www.dofus.com/fr/mmorpg/jouer");
                    //await CreateAccountMode1();

                    break;
                }
                else
                {
                    Console.WriteLine("Attente validation fin de création de compte...");
                }
                JavascriptResponse verifValidation = await browser.EvaluateScriptAsync(@"document.querySelector('#ak_field_4').value;");
                string takeValide = JsonConvert.SerializeObject(verifValidation.Result);
                if (takeValide == '\"' + "Terminer l'inscription" + '\"')
                {
                    //  int nbtryingvalidation = 0;
                    //    while (nbtryingvalidation < 5)
                    //  {
                    Console.WriteLine(takeValide);
                    Console.WriteLine("On est sur la page d'inscription, donc on réinjecte les infos !");
                    await InjecteAccountInformation();
                    //   nbtryingvalidation++;
                    // }
                }
                else
                {
                    Console.WriteLine(takeValide);
                    Console.WriteLine("On est pas sur la page d'inscription !");
                }
            }
            //JavascriptResponse takeInfoSub = await browser.EvaluateScriptAsync("document.querySelector('#challenge-form > input[type=hidden]:nth-child(1)').value;");
            //Console.WriteLine(takeInfoSub.Result.ToString());

            Thread.Sleep(200);
            Thread.Sleep(10000);
            await MakeSnapshot(0);
        }
        public async Task BypassCaptchaInscription()
        {
            string captchares;
            captchares = await HandleRecaptcha("6LfbFRsUAAAAACrqF5w4oOiGVxOsjSUjIHHvglJx", 1);

            string ckey = "";
            Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(captchares));
            if (dictionaryRes.ContainsKey("data")) //On récupère la reponse anti-captcha
            {
                ckey = (string)dictionaryRes["data"];
                captcharesult = ckey;
            }

            JavascriptResponse takeVerife;
            string takedVerife;
            bool success = true;

            Thread.Sleep(500);
            Console.WriteLine("Injecte captcha inscription...");
            Thread.Sleep(500);
            takeVerife = await browser.GetBrowser().FocusedFrame.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').style.display = " + '\'' + "block" + '\'' + ";");
            Thread.Sleep(500);
            takedVerife = JsonConvert.SerializeObject(takeVerife.Result);
            if (takedVerife == '\"' + "block" + '\"')
            {
                Console.WriteLine(takedVerife);
            }
            else
            {
                Console.WriteLine(takedVerife);
                success = false;
            }
            takeVerife = await browser.GetBrowser().FocusedFrame.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').innerHTML = " + '\'' + ckey + '\'' + ";");
            takedVerife = JsonConvert.SerializeObject(takeVerife.Result);
            Thread.Sleep(500);
            if (takedVerife != '\"'.ToString() + '\"'.ToString())
            {
                Console.WriteLine(takedVerife);
            }
            else
            {
                Console.WriteLine(takedVerife);
                success = false;
            }
            /*takeVerife = await browser.GetBrowser().FocusedFrame.EvaluateScriptAsync(@"document.querySelector('body > div.ak-mobile-menu-scroller > div > div > div:nth-child(1) > div > div > div > div.ak-inner-block > div > div.col-md-8 > div > form').submit();");
            takedVerife = JsonConvert.SerializeObject(takeVerife.Result);
            Thread.Sleep(2000);
            if (takedVerife != '\"'.ToString() + '\"'.ToString())
            {
                Console.WriteLine(takedVerife);
            }
            else
            {
                Console.WriteLine(takedVerife);
                success = false;
            }*/
            //Verife erreur
            if (success == false)
            {
                Console.WriteLine("Erreur à l'injection du captcha dans la page d'inscription.");
            }
            //Console.WriteLine("Main frame...");
            //  await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response-1').innerHTML = " + '\'' + captcharesult + '\'' + ";");
            Thread.Sleep(1000);
            Console.WriteLine("Captcha injecté mais pas soumis au formulaire. Attente de l'injection des informations de compte.");
        }
        public async Task InjecteAccountInformation()
        {
            //Def des variable attribuant les info de compte
            string userNamed = "";
            string userpass = "";
            string userMail = "";
            string dated = "1";
            string datem = "1";
            string datey = "1990";

            if (selectedAccount == 1)
            {
                userNamed = mUsername1;
                userpass = mPassword1;
                userMail = mMail1;
                dated = mDayDate1.ToString();
                datem = mMonthDate1.ToString();
                datey = mYearDate1.ToString();
            }
            else if (selectedAccount == 2)
            {
                userNamed = mUsername2;
                userpass = mPassword2;
                userMail = mMail2;
                dated = mDayDate2.ToString();
                datem = mMonthDate2.ToString();
                datey = mYearDate2.ToString();
            }
            else if (selectedAccount == 3)
            {
                userNamed = mUsername3;
                userpass = mPassword3;
                userMail = mMail3;
                dated = mDayDate3.ToString();
                datem = mMonthDate3.ToString();
                datey = mYearDate3.ToString();
            }
            else
            {
                Console.WriteLine("Erreur selection du compte !");
            }
            Console.WriteLine(userNamed);

            Console.WriteLine("On va injecter les informations de compte...");
            Console.WriteLine(string.Format("Pseudo:{0} , Password:{1} , Mail:{2} , date days:{3} , date month:{4} , date years:{5}", userNamed, userpass, userMail, dated, datem, datey));
            Thread.Sleep(1000);
            browser.GetBrowser().MainFrame.ViewSource();
            Thread.Sleep(20000);
            await browser.EvaluateScriptAsync(@"document.querySelector('body > div.ak-mobile-menu-scroller > div > div > div:nth-child(1) > div > div > div > div.ak-inner-block > div > div.col-md-8 > div > form > fieldset > div > div > div input[type=text]').value= " + '\'' + userNamed + '\'' + "; ");     //document.querySelector('#userlogin').value= " + '\'' + userNamed + '\'' + ";");
            Thread.Sleep(1000);
            await browser.EvaluateScriptAsync(@"document.getElementById('user_password').value= " + '\'' + userpass + '\'' + ";");
            Thread.Sleep(200);
            await browser.EvaluateScriptAsync(@"document.getElementById('user_password_confirm').value= " + '\'' + userpass + '\'' + ";");
            Thread.Sleep(200);
            await browser.EvaluateScriptAsync(@"document.getElementById('user_mail').value= " + '\'' + userMail + '\'' + ";");
            Thread.Sleep(200);
            await browser.EvaluateScriptAsync(@"document.getElementById('ak_field_1').value= " + '\'' + dated + '\'' + ";");
            Thread.Sleep(200);
            await browser.EvaluateScriptAsync(@"document.getElementById('ak_field_2').value= " + '\'' + datem + '\'' + ";");
            Thread.Sleep(200);
            await browser.EvaluateScriptAsync(@"document.getElementById('ak_field_3').value= " + '\'' + datey + '\'' + ";");
            Thread.Sleep(200);
            Thread.Sleep(5000);
            await MakeSnapshot(6);
            Thread.Sleep(5000);
            Console.WriteLine("On va valider la création du compte...");
            //await browser.EvaluateScriptAsync(@"document.querySelector('body > div.ak-mobile-menu-scroller > div > div > div:nth-child(1) > div > div > div > div.ak-inner-block > div > div.col-md-8 > div > form').submit();");
            await browser.EvaluateScriptAsync(@"document.querySelector('#ak_field_4').click();");
            Thread.Sleep(10000);
            await MakeSnapshot(0);

            Thread.Sleep(2000);
        }

        public async Task VerifBypassSecondCaptcha()
        {
            JavascriptResponse takeMode = await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').innerHTML;");
            string takedmode = JsonConvert.SerializeObject(takeMode.Result);
            if (takedmode != '\"'.ToString() + '\"'.ToString())
            {
                Console.WriteLine(takedmode);
                Console.WriteLine("Le captcha est bien résolut.");
            }
            else
            {
                Console.WriteLine(takedmode);
                Console.WriteLine("Le captcha doit être refait.");
                string captchares;
                captchares = await HandleRecaptcha("6LfbFRsUAAAAACrqF5w4oOiGVxOsjSUjIHHvglJx", 1);

                string ckey = "";
                Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(captchares));
                if (dictionaryRes.ContainsKey("data")) //On récupère la reponse anti-captcha
                {
                    ckey = (string)dictionaryRes["data"];
                }
                Thread.Sleep(500);
                //await browser.EvaluateScriptAsync(@"document.querySelector('#recaptcha-token').value =  " + '\'' + captcharesult + '\'' + ";");
                // await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').style.display =  " + '\'' + "block" + '\'' + ";");
                // Thread.Sleep(1000);
                await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').innerHTML = " + '\'' + ckey + '\'' + ";");
                Thread.Sleep(1000);
                Console.WriteLine("Captcha injecté mais pas soumis au formulaire.Attente de l'injection des informations de compte.");
                JavascriptResponse verifCaptchaWorkind = await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').innerHTML;");
                string verifCaptchaWorkindss = JsonConvert.SerializeObject(verifCaptchaWorkind.Result);
                Console.WriteLine(verifCaptchaWorkindss);
                mCapctha = 2;
                await CreateAccountMode2();
            }
        }



        private async void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            //Console.WriteLine(e.Browser.GetFrame(2).Url);
            if (!e.IsLoading && mBusy == false && e.Browser.MainFrame.Url == "https://www.dofus.com/fr/mmorpg/jouer")
            {
                mBusy = true;
                Thread.Sleep(4000);
                await MakeSnapshot(6);
                Thread.Sleep(2000);
                Console.WriteLine($"page {e.Browser.MainFrame.Url} loaded!");
                string postR = "";
                string postId = "";
                /*
                Thread.Sleep(6000);
                //On commence à récupérer les données captcha 
                JavascriptResponse takeInfo = await browser.EvaluateScriptAsync("document.querySelector('#challenge-form > input[type=hidden]:nth-child(1)').value;");
                postR = takeInfo.Result.ToString();
                //Console.WriteLine(takeInfo.Result.ToString());

                JavascriptResponse takeInfo2 = await browser.EvaluateScriptAsync("document.querySelector('#challenge-form > script').dataset;");

                string rawess = JsonConvert.SerializeObject(takeInfo2.Result);
                Console.WriteLine(rawess);
                Dictionary<string, object> dictionaryResds = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(rawess));
                
                if (dictionaryResds.ContainsKey("sitekey")) //On récupère l'apikey
                {
                    string monray = (string)dictionaryResds["sitekey"];
                    Console.WriteLine(monray);
                    mSitekey = monray;
                }
                else
                {
                    Console.WriteLine("Sitekey non trouver !");
                } 


                string captchares;
                captchares = await HandleRecaptcha(mSitekey, 1);

                string ckey = "";
                Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(captchares));
                if (dictionaryRes.ContainsKey("data")) //On récupère la reponse anti-captcha
                {
                    ckey = (string)dictionaryRes["data"];

                }
                Thread.Sleep(2000);
                


                await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').style.display =  " + '\'' + "block" + '\'' + ";");
                Thread.Sleep(2000);
                await browser.EvaluateScriptAsync(@"document.querySelector('#g-recaptcha-response').innerHTML = " + '\'' + ckey + '\'' + ";");
                Thread.Sleep(5000);
                await MakeSnapshot(0);
                Thread.Sleep(5000);
                await browser.EvaluateScriptAsync(@"document.querySelector('#challenge-form input[type=submit]').click();");
                Thread.Sleep(5000);
                await MakeSnapshot(1);
                Console.WriteLine("Requête terminer !");
                Thread.Sleep(5000);

                await MakeSnapshot(6);
                Thread.Sleep(5000);
                Console.WriteLine("Normalement on a passez le captcha !");

                while (e.IsLoading && e.Browser.MainFrame.Url.Contains("https://www.dofus.com/fr/mmorpg/jouer?__cf_chl_captcha_tk__=") == false)
                {

                }
                */
                Thread.Sleep(5000);
                browser.SetZoomLevel(-2);
                Thread.Sleep(2000);
                string userNamed = "jackouille99700";
                string userpass = "CHANGE_ME";
                string userMail = "mailbox@example.com";
                string dated = "25";
                string datem = "3";
                string datey = "1992";
                Console.WriteLine("On va injecter les informations de compte...");
                Console.WriteLine(string.Format("Pseudo:{0} , Password:{1} , Mail:{2} , date days:{3} , date month:{4} , date years:{5}", userNamed, userpass, userMail, dated, datem, datey));
                Thread.Sleep(1000);
                await browser.EvaluateScriptAsync(@"document.getElementById('userlogin').value= " + '\'' + userNamed + '\'' + ";");
                Thread.Sleep(200);
                await browser.EvaluateScriptAsync(@"document.getElementById('user_password').value= " + '\'' + userpass + '\'' + ";");
                Thread.Sleep(200);
                await browser.EvaluateScriptAsync(@"document.getElementById('user_password_confirm').value= " + '\'' + userpass + '\'' + ";");
                Thread.Sleep(200);
                await browser.EvaluateScriptAsync(@"document.getElementById('user_mail').value= " + '\'' + userMail + '\'' + ";");
                Thread.Sleep(200);
                await browser.EvaluateScriptAsync(@"document.getElementById('ak_field_1').value= " + '\'' + dated + '\'' + ";");
                Thread.Sleep(200);
                await browser.EvaluateScriptAsync(@"document.getElementById('ak_field_2').value= " + '\'' + datem + '\'' + ";");
                Thread.Sleep(200);
                await browser.EvaluateScriptAsync(@"document.getElementById('ak_field_3').value= " + '\'' + datey + '\'' + ";");
                Thread.Sleep(200);
                Thread.Sleep(5000);
                await MakeSnapshot(6);
                Thread.Sleep(5000);
                Console.WriteLine("On va valider la création du compte...");
                await browser.EvaluateScriptAsync(@"document.querySelector('body > div.ak-mobile-menu-scroller > div > div > div:nth-child(1) > div > div > div > div.ak-inner-block > div > div.col-md-8 > div > form > fieldset > div > div > div > div.row.ak-container > div input[type=submit]').click();");
                Thread.Sleep(5000);
                //JavascriptResponse takeInfoSub = await browser.EvaluateScriptAsync("document.querySelector('#challenge-form > input[type=hidden]:nth-child(1)').value;");
                //Console.WriteLine(takeInfoSub.Result.ToString());
                await MakeSnapshot(0);




                captchabypassed = true;
                mBusy = false;

            }
            else
            {
                Console.WriteLine($"page {e.Browser.FocusedFrame.Url} is loading!");
            }
        }



        public async Task GetReturnChrom(object sender, FrameLoadEndEventArgs e)
        {
            //On écrit l'url ou on est.
            Console.WriteLine(e.Url);
            Console.WriteLine("Chargement de la page...");
            string script = "";
            /*  while (browser.IsLoading)
              {

              }*/


            //On vérifie se que contient la frame reçue
            if (e.Url.Contains("about"))
            {
                Console.WriteLine("Page about:blank charger ");
            }
            else if (e.Url.Contains("recaptcha/api2/anchor"))
            {
                if (mCkey == "" && captchawork == false)
                {
                    Console.WriteLine(e.Url);
                    captchawork = true;
                    captchamethode = "anchor";
                    /*
                    string code;
                    code = await e.Frame.GetSourceAsync();
                    Console.WriteLine(code);
                    */
                    string captchares;
                    captchares = await HandleRecaptcha(mSitekey, 1);
                    string ckey = "";
                    Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(captchares));
                    if (dictionaryRes.ContainsKey("data")) //On récupère l'apikey
                    {
                        ckey = (string)dictionaryRes["data"];

                    }


                    Console.WriteLine("On va effectuer la requête pour la methode anchor !");
                    script = @"document.querySelector('#g-recaptcha-response').style.display = 'block'" + "; " +
                    "document.querySelector('#g-recaptcha-response').innerHTML = " + '\'' + ckey + '\'' + ";" +
                    "document.querySelector('#recaptcha_submit').click();"
                    ;
                    //await MakeSnapshot(2);
                    //browser.GetBrowser().GetFrame("").EvaluateScriptAsync(script).ContinueWith(x =>
                    e.Frame.EvaluateScriptAsync(script).ContinueWith(x =>
                   {
                       var response = x.Result;
                       var startDate = response.Result;
                       Console.WriteLine(startDate);

                   });
                    Console.WriteLine("Requête terminer !");
                    Thread.Sleep(15000);
                    Console.WriteLine(e.Frame.Url);
                    await MakeSnapshot(6);
                    Thread.Sleep(15000);
                    Console.WriteLine("Normalement on a passez le captcha !");


                    captchabypassed = true;

                    //await MakeSnapshot(5);
                    /*
                    Console.WriteLine("On va effectuer la requête pour la methode anchor !");
                    script = @"document.getElementById('g-recaptcha-response').value = " + '\'' + ckey + '\'' + ";" +
                    "document.querySelector('input[type=submit]').click();"
                    ;
                    //await MakeSnapshot(2);
                    //browser.GetBrowser().GetFrame("").EvaluateScriptAsync(script).ContinueWith(x =>
                    await e.Frame.EvaluateScriptAsync(script, e.Url).ContinueWith(x =>
                    {
                        var response = x.Result;
                        var startDate = response.Result;
                        Console.WriteLine(startDate);

                    });
                    Console.WriteLine("Requête terminer !");
                    Thread.Sleep(5000);
                    Console.WriteLine("Normalement on a passez le captcha !");
                    Console.WriteLine(e.Frame.Url);
                    captchabypassed = true;
                     */


                    mCkey = ckey;
                    //e.Frame.Delete();
                }
                //  await Task.Delay(5000);
                // Console.WriteLine(captchares);
                // string scriptcapt = string.Format("document.getElementById('g-recaptcha-response').innerHTML = " + '\'' + captchares + '\'' + ";");
                // e.Frame.ExecuteJavaScriptAsync(scriptcapt, e.Url);
            }
            else if (e.Url.Contains("recaptcha/api2/bframe looodfkkdoskfokoezkofkoezkfokezo"))
            {
                if (mCkey == "" && captchawork == false)
                {
                    Console.WriteLine(e.Url);
                    captchawork = true;
                    captchamethode = "bframe";
                    await MakeSnapshot(2);
                    string captchares;
                    captchares = await HandleRecaptcha(mSitekey, 1);

                    Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(captchares));
                    if (dictionaryRes.ContainsKey("data")) //On récupère l'apikey
                    {
                        string ckey = (string)dictionaryRes["data"];
                        mCkey = ckey;
                    }
                    await MakeSnapshot(3);
                    string code;
                    code = await e.Frame.GetSourceAsync();
                    e.Frame.Delete();
                }
                //  await Task.Delay(5000);
                // Console.WriteLine(captchares);
                // string scriptcapt = string.Format("document.getElementById('g-recaptcha-response').innerHTML = " + '\'' + captchares + '\'' + ";");
                // e.Frame.ExecuteJavaScriptAsync(scriptcapt, e.Url);


            }
            else if (e.Url.Contains("recaptcha/api/fallback")) //https://www.google.com/recaptcha/api/fallback?k=6LfBixYUAAAAABhdHynFUIMA_sa4s-XsJvnjtgB0
            {
                if (mCkey == "" && captchawork == false)
                {
                    Console.WriteLine(e.Url);
                    captchawork = true;
                    await MakeSnapshot(2);
                    captchamethode = "fallback";
                    string captchares;
                    captchares = await HandleRecaptcha(mSitekey, 1);

                    Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(captchares));
                    if (dictionaryRes.ContainsKey("data")) //On récupère l'apikey
                    {
                        string ckey = (string)dictionaryRes["data"];
                        mCkey = ckey;
                    }
                    /*
                    string code;
                    code = await e.Frame.GetSourceAsync();
                    
                    Console.Write(code);
                    */
                    await MakeSnapshot(3);
                    e.Frame.Delete();
                }
                //  await Task.Delay(5000);
                // Console.WriteLine(captchares);
                // string scriptcapt = string.Format("document.getElementById('g-recaptcha-response').innerHTML = " + '\'' + captchares + '\'' + ";");
                // e.Frame.ExecuteJavaScriptAsync(scriptcapt, e.Url);


            }
            else if (e.Url.Contains("dofus"))
            {
                while (mCkey == "")
                {

                }


                if (captchamethode == "fallback" && captchabypassed == false)
                {
                    captchabypassed = true;
                    Console.WriteLine("On va effectuer la requête pour la methode fallback !");
                    string scriptcapt = @"document.querySelector('input[type=hidden]').value = " + '\'' + mCkey + '\'' + ";" +
                    "document.querySelector('input[type=submit]').click();"
                    ;

                    await e.Frame.EvaluateScriptAsync(scriptcapt, e.Frame.Url).ContinueWith(x =>
                     {
                         var response = x.Result;

                         if (response.Success && response.Result != null)
                         {
                             var startDate = response.Result;
                             //Console.WriteLine("On entre le pseudo");
                             //Console.WriteLine(startDate);
                             //startDate is the value of a HTML element.

                         }
                     });
                    Console.WriteLine("Requête terminer !");
                    Thread.Sleep(5000);
                    Console.WriteLine("Normalement on a passez le captcha !");
                }
                if (captchamethode == "anchor" && captchabypassed == false)
                {

                }



                string userNamed = "jackouille99700";
                string userpass = "CHANGE_ME";
                string userMail = "mailbox@example.com";
                string dated = "25";
                string datem = "3";
                string datey = "1992";

                Console.WriteLine("On arrive a la page dofus !!!");
                Thread.Sleep(10000);
                await MakeSnapshot(0);

                browser.SetZoomLevel(-2);

                //await Task.Delay(10000);
                /*while (mCkey == "")
                {

                }
                */

                /*
                
                await Task.Delay(5000);
                string scriptcapt = string.Format("document.getElementById('g-recaptcha-response').value = " + '\'' + mCkey + '\'' + ";");
                await e.Frame.EvaluateScriptAsync(scriptcapt, e.Url).ContinueWith(x =>
                {
                    var response = x.Result;

                    if (response.Success && response.Result != null)
                    {
                        var startDate = response.Result;
                        Console.WriteLine("On entre le captchat");
                        Console.WriteLine(startDate);
                        //startDate is the value of a HTML element.

                    }
                });
                */
                Console.WriteLine("Vient d'injecter le captchat");

                Console.WriteLine(mCkey);

                //string script = string.Format("document.getElementById('userlogin').value= " + '\'' + userNamed + '\'' + ";");
                script = @"document.getElementById('userlogin').value= " + '\'' + userNamed + '\'' + "; " +
                   "document.getElementById('user_password').value= " + '\'' + userpass + '\'' + ";" +
                   "document.getElementById('user_password_confirm').value= " + '\'' + userpass + '\'' + ";" +
                   "document.getElementById('user_mail').value= " + '\'' + userMail + '\'' + ";" +
                   "document.getElementById('ak_field_1').value= " + '\'' + dated + '\'' + ";" +
                   "document.getElementById('ak_field_2').value= " + '\'' + datem + '\'' + ";" +
                   "document.getElementById('ak_field_3').value= " + '\'' + datey + '\'' + ";" +
                   "document.querySelector('body > div.ak-mobile-menu-scroller > div > div > div:nth-child(1) > div > div > div > div.ak-inner-block > div > div.col-md-8 > div > form > fieldset > div > div > div > div.row.ak-container > div input[type=submit]').click();"
                   ;
                // "document.querySelector('#g-recaptcha-response').value = " + '\'' + mCkey + '\'' + ";" +

                //string.Format("document.querySelector('#user_password').value= 'CHANGE_ME';");

                await e.Frame.EvaluateScriptAsync(script).ContinueWith(x =>
                {
                    var response = x.Result;

                    if (response.Success && response.Result != null)
                    {
                        var startDate = response.Result;
                        Console.WriteLine("On entre le pseudo");
                        Console.WriteLine(startDate);
                        //startDate is the value of a HTML element.

                    }

                });

                Console.WriteLine("On a entrer le pseudo");
                Thread.Sleep(7000);
                /*
                // string script2 = string.Format("document.getElementById('user_password').value= " + '\'' + userpass + '\'' + ";");
                string script2 = string.Format("document.querySelector('#user_password').value= 'CHANGE_ME';");

                await e.Frame.EvaluateScriptAsync(script2).ContinueWith(a =>
                {
                    var response2 = a.Result;

                    if (response2.Success && response2.Result != null)
                    {
                        var startDate2 = response2.Result;
                        Console.WriteLine("On entre le password");
                        Console.WriteLine(startDate2);
                        //startDate is the value of a HTML element.

                    }
                });
                Console.WriteLine("On a entrer le password");
                await MakeSnapshot(0);
                string submit = string.Format("document.getElementById('ak_field_4');");
                await e.Frame.EvaluateScriptAsync(submit, e.Url).ContinueWith(x =>
                {
                    var response = x.Result;

                    if (response.Success && response.Result != null)
                    {
                        var startDate = response.Result;
                        Console.WriteLine("On valide");
                        Console.WriteLine(startDate);
                        //startDate.GetType();
                        //Console.WriteLine(startDate);
                        //startDate is the value of a HTML element.
                    }
                });
                
                await MakeSnapshot(0);
                await MakeSnapshot(1);
                */
                await MakeSnapshot(1);
            }
            /*
        await e.Frame.GetTextAsync().ContinueWith(taskHtml => //On lit ici la réponse
        {
            try
            {
                string html = taskHtml.Result; //réponse
                                               //La réponse est en JSON, on en crée un dictionnaire
                Dictionary<string, object> dictionaryRes = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(html));

                Console.WriteLine("Lecture de la frame reçue !");

                Console.WriteLine(html);
                Console.WriteLine(dictionaryRes);

                return e.HttpStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lecture de la frame reçue !");
                Console.WriteLine(e.HttpStatusCode);
                return e.HttpStatusCode;
            }
        });
        */
            /*
            if (e.Url.Contains("haapi")) //Si c'est une FRAME demander l'apikey
            {
                if (e.HttpStatusCode == 200) //Si la requête POST a fonctionné --> on continue 
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
                        _apiKey = "failed";
                    });

                }
                else //Si la requête a été interdit .. à traiter : (HttpStatusCode... = 403)
                {
                    _apiKey = "failed";
                    return e.HttpStatusCode;
                }
            }
            */
        }



        public async Task<String> HandleRecaptcha(string sitekey, int tries = 1)
        {

            // If _wasScriptRunning was already true, don't change it        
            RecaptchaReceived?.Invoke(this);

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                Console.WriteLine("reCaptcha Getting response..");
                // string response = RecaptchaHandler.GetResponse(sitekey);
                string response = "";
                RecaptchaHandler cpttask = new RecaptchaHandler();
                response = await cpttask.GetResponse(sitekey);
                Console.WriteLine("reCaptcha Got response.");

                // If the response is null, its because the user didn't enter an anti-captcha key
                if (response == null)
                {
                    // We shouldn't leave this True
                    Console.WriteLine("Erreur Anti-captcha personnal key !");
                    RecaptchaResolved?.Invoke(this, false);
                }
                else
                {
                    Console.Write("Réponce reçue en ");
                    Console.Write(sw.Elapsed.TotalSeconds);
                    Console.Write(" secondes.");
                    Console.WriteLine(" ");

                    dynamic msg = new ExpandoObject();
                    msg.call = "recaptchaResponse";
                    msg.data = response;
                    string raw = JsonConvert.SerializeObject(msg);

                    Console.Write("Clée reçue =");
                    Console.Write(raw);
                    Console.WriteLine(" ");

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
            catch (Exception ex)
            {
                Console.Write("Erreur reCaptcha");
                Console.Write(ex.Message);
                if (tries < 3)
                {
                    Console.Write("Captcha non résolut tentative numéro ");
                    Console.Write(++tries);
                    Console.WriteLine(" ");
                    await HandleRecaptcha(sitekey, tries);
                }

            }
            return "Captcha non résolut !";
        }


        private async Task MakeSnapshot(int number)
        {

            // Remove the load event handler, because we only want one snapshot of the initial page.
            //browser.LoadingStateChanged -= BrowserLoadingStateChanged;

            //  var scriptTask = browser.GetMainFrame(); //.EvaluateScriptAsync("document.getElementById('lst-ib').value = 'CefSharp Was Here!'");

            //Give the browser a little time to render
            Thread.Sleep(500);
            // Wait for the screenshot to be taken.
            var task = browser.ScreenshotAsync();
            await task.ContinueWith(x =>
            {
                // Make a file to save it to (e.g. C:\Users\dev\Desktop\CefSharp screenshot.png)
                var screenshotPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DebugJackouille" + number + ".png");

                //Console.WriteLine();
                //Console.WriteLine("Screenshot ready. Saving to {0}", screenshotPath);

                // Save the Bitmap to the path.
                // The image type is auto-detected via the ".png" extension.
                task.Result.Save(screenshotPath);

                // We no longer need the Bitmap.
                // Dispose it to avoid keeping the memory alive.  Especially important in 32-bit applications.
                task.Result.Dispose();

                /*
                // Tell Windows to launch the saved image.
                Process.Start(new ProcessStartInfo(screenshotPath)
                {
                    // UseShellExecute is false by default on .NET Core.
                    UseShellExecute = true
                });

                Console.WriteLine("Image viewer launched.  Press any key to exit.");
                */
            }, TaskScheduler.Default);
            Console.WriteLine("Fin screen");

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




















    }
}

