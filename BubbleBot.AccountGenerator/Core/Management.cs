using CefSharp;
using CefSharp.OffScreen;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AccountGenerator.Core
{
    public class ManagementGeneration
    {
        //Les fichier de config du programme 
        private static string proxyFile = @"\proxy.txt";
#pragma warning disable CS0414 // The field 'ManagementGeneration.configFile' is assigned but its value is never used
        private static string configFile = @"\config.txt";
#pragma warning restore CS0414 // The field 'ManagementGeneration.configFile' is assigned but its value is never used
        private static string outputFile = @"\account.txt";
        private static string debugFile = @"\debug.txt";
        private static string mailFile = @"\credentials.json";

        private SemaphoreSlim _semaphoreFile = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _semaphoreDebugFile = new SemaphoreSlim(1, 1);

        //Definition des variables nécessaire a la vérification des proxys
        private static List<string> proxyList = null;
        private string[] alowedCountry = { "BD", "BE", "BJ", "MM", "BO", "CM", "CA", "CY", "FR", "GB", "IQ", "JP", "PG", "PY", "PR", "PE", "SV", "SD", "PS", "LK" };
        private SemaphoreSlim _semaphoreProxy = new SemaphoreSlim(1, 1);

        //PROGRAME CONFIGURATION
        private int MAX_THREAD = Program.MAX_THREAD;           //Nombre maximum de thread simultané
        private int MODE = Program.PROGRAM_MODE;                  //Mode d'execution du programme 
        private int PROXY_NBTRY = Program.MAX_TRYPROXY;
        private const int OUTPUTCHECKEDPROXY = 1;    //Permet d'ajouter les proxys verifier a un fichier txt

        //Account configuration
        private static string PASSWORD = Program.ACCOUNT_PASSWORD;
        private static string MAIL = Program.ACCOUNT_MAIL;

        //Get personnal param
        private int counterProxy = Program.PROXY_COUNTER;

        //Mail management 
        private string lastMail = "";
        private List<string> allUrlValidation = new List<string>();

        //Multiple account Management 
        private int proxyFailled = 0;
        private SemaphoreSlim _semaphoreMultipleProxy = new SemaphoreSlim(1, 1);
        private List<string> proxyCertified = new List<string>();
        private int maxProxy = 0;

        public async Task GeneratorManagement()
        {
            //  Console.WriteLine(Directory.GetCurrentDirectory()+ mailFile);
            //  System.Threading.Thread.Sleep(1000000);
            if (File.Exists(Directory.GetCurrentDirectory() + proxyFile))
            {
                proxyList = File.ReadAllLines(Directory.GetCurrentDirectory() + proxyFile).ToList();
                if (proxyList.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Les proxys ont bien été trouver !");
                    Console.WriteLine("Found " + proxyList.Count + " proxys.");
                    maxProxy = proxyList.Count - 1;
                    // await StartGeneration();
                    if (MODE == 1)
                    {
                        await StartGenerationMultiple();
                    }
                    else
                    {
                        //await MailVerification(false);
                        //await MailValidation(allUrlValidation);
                        //allUrlValidation = new List<string>();
                        await StartGeneration();
                    }

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Erreur, la liste de proxy est vide !");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur, les proxys n'ont pas été trouver !");
            }

        }
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task debugOutput(string debugtxt)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            _semaphoreDebugFile.Wait();
            if (File.Exists(Directory.GetCurrentDirectory() + debugFile))
            {
            }
            else
            {
                File.Create(Directory.GetCurrentDirectory() + debugFile);
            }
            StreamWriter file = new StreamWriter(Directory.GetCurrentDirectory() + debugFile, true);
            file.Write(debugtxt);
            file.Flush();
            file.Dispose();
            _semaphoreDebugFile.Release();
        }

        public async Task StartGenerationMultiple()
        {
            Console.WriteLine("Debut de la generation...");
            int nbaccount = 0;
            int ingen = 0;
            bool accountcreated = false;
            await MailVerification(true);

            while (true)
            {
                accountcreated = false;
                List<AccountGeneratorTouch> currentGen = new List<AccountGeneratorTouch>();

                if (counterProxy > maxProxy)
                {
                    Console.WriteLine("Fin de la génération...");
                    break;
                }

                for (int i = 0; i < MAX_THREAD; i++)
                {
                    if (counterProxy > maxProxy)
                    {
                        MAX_THREAD = i;
                        break;
                    }
                    else
                    {
                        //      Console.WriteLine("Lancement proxy toute thread...");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                        ProxyChecker(proxyList.ElementAt(counterProxy));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                        counterProxy++;
                    }
                }

                while (proxyCertified.Count < MAX_THREAD)
                {
                    //  Console.WriteLine("Verif proxy toute thread...");
                    if (counterProxy > maxProxy && proxyFailled > 0)
                    {
                        proxyFailled--;
                        MAX_THREAD--;
                    }
                    else
                    {
                        if (proxyFailled > 0)
                        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                            ProxyChecker(proxyList.ElementAt(counterProxy));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                            counterProxy++;
                            proxyFailled--;
                        }
                    }
                }

                for (int i = 0; i < MAX_THREAD; i++)
                {
                    currentGen.Add(new AccountGeneratorTouch("", PASSWORD, MAIL, proxyCertified.ElementAt(i), "", nbaccount));
                    ingen++;
                }

                for (int i = 0; i < MAX_THREAD; i++)
                {
                    await Task.Run(() => currentGen.ElementAt(i).CreationCompteStart()).ConfigureAwait(false);
                    await Task.Delay(1000);
                }

                while (ingen > 0)
                {
                    for (int i = 0; i < MAX_THREAD; i++)
                    {
                        if (currentGen.ElementAt(i).allfinished == true)
                        {
                            currentGen.ElementAt(i).allfinished = false;
                            ingen--;
                        }
                    }
                    await Task.Delay(1000);
                }

                for (int i = 0; i < MAX_THREAD; i++)
                {
                    string resAccount1 = currentGen.ElementAt(i).outputAcc1;
                    string resAccount1p = currentGen.ElementAt(i).outputAcc1p;
                    string resAccount2 = currentGen.ElementAt(i).outputAcc2;
                    string resAccount2p = currentGen.ElementAt(i).outputAcc2p;
                    string resAccount3 = currentGen.ElementAt(i).outputAcc3;
                    string resAccount3p = currentGen.ElementAt(i).outputAcc3p;
                    string resDebug = currentGen.ElementAt(i).debug;
                    await debugOutput(resDebug);

                    if (resAccount1 != "" && resAccount1p != "")
                    {
                        accountcreated = true;
                        nbaccount++;
                        await outputAccount(resAccount1, resAccount1p);
                    }
                    if (resAccount2 != "" && resAccount2p != "")
                    {
                        nbaccount++;
                        await outputAccount(resAccount2, resAccount2p);
                    }
                    if (resAccount3 != "" && resAccount3p != "")
                    {
                        nbaccount++;
                        await outputAccount(resAccount3, resAccount3p);
                    }
                }

                if (accountcreated == true)
                {
                    await MailVerification(false);
                    await MailValidation(allUrlValidation);
                    allUrlValidation = new List<string>();
                }

                ingen = 0;
                accountcreated = false;
                proxyCertified = new List<string>();
            }

        }

        public async Task StartGeneration()
        {
            Console.WriteLine("Debut de la generation...");
            int nbaccount = 0;
            await MailVerification(true);

            while (true)
            {
                if (proxyList.ElementAt(counterProxy) == "")
                {
                    Console.WriteLine("Fin de la génération...");
                    break;
                }

                while (await ProxyChecker(proxyList.ElementAt(counterProxy)) == false)
                {
                    counterProxy++;
                }

                var accountCreating = new AccountGeneratorTouch("", PASSWORD, MAIL, proxyList.ElementAt(counterProxy), "", nbaccount);
                accountCreating.CreationCompteStart();
                while (accountCreating.allfinished == false)
                {
                    await Task.Delay(1000);
                }

                //Console.WriteLine("Fin de la creation du compte: 1");
                string resAccount1 = accountCreating.outputAcc1;
                string resAccount1p = accountCreating.outputAcc1p;
                string resAccount2 = accountCreating.outputAcc2;
                string resAccount2p = accountCreating.outputAcc2p;
                string resAccount3 = accountCreating.outputAcc3;
                string resAccount3p = accountCreating.outputAcc3p;
                string resDebug = accountCreating.debug;
                await debugOutput(resDebug);

                if (resAccount1 != "" && resAccount1p != "")
                {
                    nbaccount++;
                    await outputAccount(resAccount1, resAccount1p);
                }
                if (resAccount2 != "" && resAccount2p != "")
                {
                    nbaccount++;
                    await outputAccount(resAccount2, resAccount2p);
                }
                if (resAccount3 != "" && resAccount3p != "")
                {
                    nbaccount++;
                    await outputAccount(resAccount3, resAccount3p);
                }

                if (resAccount1 != "")
                {
                    await MailVerification(false);
                    await MailValidation(allUrlValidation);
                    allUrlValidation = new List<string>();
                }


                counterProxy++;
            }

        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task outputAccount(string name, string password)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            _semaphoreFile.Wait();
            if (File.Exists(Directory.GetCurrentDirectory() + outputFile))
            {
                //   Console.WriteLine("Le fichier account.txt existe déjà !");
            }
            else
            {
                File.Create(Directory.GetCurrentDirectory() + outputFile);
                //    Console.WriteLine("on creer le fichier acccount.txt !");
            }
            StreamWriter file = new StreamWriter(Directory.GetCurrentDirectory() + outputFile, true);
            file.WriteLine(name + ":" + password);
            file.Flush();
            file.Dispose();
            _semaphoreFile.Release();
        }



        #region PROXY_MANAGEMENT
        public async Task AddCertifiedProxy(string proxyVerifier)
        {
            _semaphoreProxy.Wait();
            proxyCertified.Add(proxyVerifier);
            Console.WriteLine("Nombre total de proxy verifer : {0}", proxyCertified.Count);
            _semaphoreProxy.Release();
        }

        public async Task<bool> ProxyChecker(string proxy)
        {
            //bool proxyOpen = false;
            //bool proxyChecked = false;
            // string proxyStatut = "";
            // HttpClient httpClient;
            // HttpResponseMessage resp;
            await AddCertifiedProxy(proxy);
            return true;
            /*  if (!string.IsNullOrWhiteSpace(proxy))
              {

                      int nbTries = PROXY_NBTRY;
                      Console.ForegroundColor = ConsoleColor.Blue;
                      Console.WriteLine("Vérification du proxy {0} !", proxy);
                      WebProxy webproxy = new WebProxy(proxy, false);
                      HttpClientHandler httpClientHandler = new HttpClientHandler()
                      {
                          Proxy = (IWebProxy)webproxy,
                          PreAuthenticate = false,
                          UseDefaultCredentials = false
                      };
                      while (nbTries > 0)
                      {
                          try
                          {
                              httpClient = new HttpClient(httpClientHandler);
                              resp = await httpClient.GetAsync("https://hidemyna.me/api/geoip.php?out=js&htmlentities");
                              proxyStatut = await resp.Content.ReadAsStringAsync();
                              Console.ForegroundColor = ConsoleColor.Green;
                              //Console.WriteLine(proxyStatut);
                              proxyOpen = true;
                              break;
                          }
                          catch (Exception ex)
                          {
                              Console.ForegroundColor = ConsoleColor.Red;
                              Console.WriteLine("Echec connexion au proxy, tentative restante: {0}", nbTries - 1);
                          }
                          nbTries--;
                      }


                   if (proxyOpen == true)
                   {
                       for (int i = 0; i < alowedCountry.Length; i++)
                       {
                           if (proxyStatut.Contains(alowedCountry[i]))
                           {
                               proxyChecked = true;
                           }
                       }
                       if (proxyChecked == true)
                       {
                           Console.ForegroundColor = ConsoleColor.Green;
                           Console.WriteLine("Proxy valide !");
                           if (MODE == 1)
                           {
                               await AddCertifiedProxy(proxy);
                           }
                           return true;
                       }
                       else
                       {
                           Console.ForegroundColor = ConsoleColor.Red;
                           Console.WriteLine("Erreur, pays du proxy invalide !");
                           if (MODE == 1)
                           {
                               // await AddCertifiedProxy(proxy);
                               proxyFailled++; //Peut être besoin du semaphore en cas de probleme
                           }
                           return false;
                       }
                   }
                   else
                   {
                       Console.ForegroundColor = ConsoleColor.Red;
                       Console.WriteLine("Erreur, proxy invalide !");
                       if (MODE == 1)
                       {
                           // await AddCertifiedProxy(proxy);
                           proxyFailled++; //Peut être besoin du semaphore en cas de probleme
                       }
                       return false;
                   }
               }
               return false;
               */
        }

        /*
        public async Task<bool> ProxyChecker(string proxy)
        {
            bool proxyOpen = false;
            bool proxyChecked = false;
            string proxyStatut = "";
            HttpClient httpClient;
            HttpResponseMessage resp;
            if (!string.IsNullOrWhiteSpace(proxy))
            {
                int nbTries = PROXY_NBTRY;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Vérification du proxy {0} !", proxy);
                WebProxy webproxy = new WebProxy(proxy, false);
                HttpClientHandler httpClientHandler = new HttpClientHandler()
                {
                    Proxy = (IWebProxy)webproxy,
                    PreAuthenticate = false,
                    UseDefaultCredentials = false
                };
                while (nbTries > 0)
                {
                    try
                    {
                        httpClient = new HttpClient(httpClientHandler);
                        resp = await httpClient.GetAsync("https://hidemyna.me/api/geoip.php?out=js&htmlentities");
                        proxyStatut = await resp.Content.ReadAsStringAsync();
                        Console.ForegroundColor = ConsoleColor.Green;
                        //Console.WriteLine(proxyStatut);
                        proxyOpen = true;
                        break;
                    }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                    catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Echec connexion au proxy, tentative restante: {0}", nbTries - 1);
                    }
                    nbTries--;
                }
                if (proxyOpen == true)
                {
                    for (int i = 0; i < alowedCountry.Length; i++)
                    {
                        if (proxyStatut.Contains(alowedCountry[i]))
                        {
                            proxyChecked = true;
                        }
                    }
                    if (proxyChecked == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Proxy valide !");
                        if (MODE == 1)
                        {
                            await AddCertifiedProxy(proxy);
                        }
                        return true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Erreur, pays du proxy invalide !");
                        if (MODE == 1)
                        {
                            // await AddCertifiedProxy(proxy);
                            proxyFailled++; //Peut être besoin du semaphore en cas de probleme
                        }
                        return false;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Erreur, proxy invalide !");
                    if (MODE == 1)
                    {
                        // await AddCertifiedProxy(proxy);
                        proxyFailled++; //Peut être besoin du semaphore en cas de probleme
                    }
                    return false;
                }
            }
            return false;
        }
        */
        #endregion PROXY_MANAGEMENT

        #region MAIL_MANAGEMENT


        public async Task MailValidation(List<string> mailList)
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

            var mailBrowser = new ChromiumWebBrowser("about:blank", browserSettings, new RequestContext());


            var browserInit = SpinWait.SpinUntil(() => mailBrowser.IsBrowserInitialized, TimeSpan.FromSeconds(30));
            if (!browserInit)
            {
                Console.WriteLine("Mail browser error.");
            }

            Console.WriteLine("Mail validation...");
            foreach (string urlToVal in mailList)
            {
                Console.WriteLine(urlToVal);
                mailBrowser.Load(urlToVal);

                while (mailBrowser.IsLoading)
                {
                    await Task.Delay(1000);
                }
                Console.WriteLine(await mailBrowser.GetMainFrame().GetTextAsync());
            }

            mailBrowser.Dispose();
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task MailVerification(bool modedate)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {

            List<string> tempUrlValidation = new List<string>();

            try
            {
                // If modifying these scopes, delete your previously saved credentials
                // at ~/.credentials/gmail-dotnet-quickstart.json
                string[] Scopes = { GmailService.Scope.GmailModify };
                string ApplicationName = "Gmail API .NET Quickstart";

                UserCredential credential;
                //Modifier le chemin
                using (var stream =
                    new FileStream(Directory.GetCurrentDirectory() + mailFile, FileMode.Open, FileAccess.Read))
                {
                    string credPath = System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart2.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    //  Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Gmail API service.
                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });


                var re = service.Users.Messages.List("me");
                re.LabelIds = "INBOX";
                // re.Q = "is:unread"; //only get unread;

                var res = re.Execute();

                if (res != null && res.Messages != null)
                {
                    // Console.WriteLine("there are {0} emails. press any key to continue!", res.Messages.Count);
                    //  Console.ReadKey();

                    foreach (var email in res.Messages)
                    {
                        var emailInfoReq = service.Users.Messages.Get("me", email.Id);
                        var emailInfoResponse = emailInfoReq.Execute();

                        if (emailInfoResponse != null)
                        {
                            String from = "";
                            String date = "";
                            String subject = "";
                            String body = "";
                            //loop through the headers and get the fields we need...
                            foreach (var mParts in emailInfoResponse.Payload.Headers)
                            {
                                if (mParts.Name == "Date")
                                {
                                    date = mParts.Value;
                                }
                                else if (mParts.Name == "From")
                                {
                                    from = mParts.Value;
                                }
                                else if (mParts.Name == "Subject")
                                {
                                    subject = mParts.Value;
                                }

                                if (date != "" && from != "")
                                {
                                    if (modedate)
                                    {
                                        lastMail = date;
                                        Console.WriteLine("On a enregistrer la date du dernier mail reçue: {0}", lastMail);
                                        return;
                                    }

                                    if (emailInfoResponse.Payload.Parts == null && emailInfoResponse.Payload.Body != null)
                                        body = DecodeBase64String(emailInfoResponse.Payload.Body.Data);
                                    else
                                        body = GetNestedBodyParts(emailInfoResponse.Payload.Parts, "");

                                    //now you have the data you want....

                                }

                            }
                            //Console.WriteLine(body);
                            //Console.WriteLine("Decoupe address mail validation");
                            if (date != lastMail)
                            {
                                //Console.WriteLine("Chek value validation");
                                if (body.Contains("https://www.dofus-touch.com/fr/mmorpg/jouer?guid="))
                                {
                                    string posUrl1 = "https://www.dofus-touch.com/fr/mmorpg/jouer?guid=";
                                    int Index1 = body.IndexOf(posUrl1);
                                    int Index2 = body.IndexOf(@" ]", Index1);
                                    string validUrl = body.Substring(Index1, Index2 - Index1);
                                    Console.WriteLine(validUrl);
                                    tempUrlValidation.Add(validUrl);
                                }
                            }
                            else
                            {
                                break;
                            }

                            //Console.Write(body);
                            //Console.WriteLine("{0}  --  {1}  -- {2} ---{3}", subject, date, email.Id, body);
                            //Console.ReadKey();
                        }
                    }
                }
                //Relancer la fonction si aucun mail n'a été reçue .

                //Supprime les doublons de la liste
                foreach (string vals in tempUrlValidation)
                {
                    bool chcecked = false;
                    foreach (string resu in allUrlValidation)
                    {
                        if (resu == vals)
                        {
                            chcecked = true;
                        }
                    }
                    if (chcecked == false)
                    {
                        allUrlValidation.Add(vals);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get messages!   {0}", e);
            }

        }



        static String DecodeBase64String(string s)
        {
            var ts = s.Replace("-", "+");
            ts = ts.Replace("_", "/");
            var bc = Convert.FromBase64String(ts);
            var tts = Encoding.UTF8.GetString(bc);

            return tts;
        }

        static String GetNestedBodyParts(IList<MessagePart> part, string curr)
        {
            string str = curr;
            if (part == null)
            {
                return str;
            }
            else
            {
                foreach (var parts in part)
                {
                    if (parts.Parts == null)
                    {
                        if (parts.Body != null && parts.Body.Data != null)
                        {
                            var ts = DecodeBase64String(parts.Body.Data);
                            str += ts;
                        }
                    }
                    else
                    {
                        return GetNestedBodyParts(parts.Parts, str);
                    }
                }

                return str;
            }
        }

        #endregion MAIL_MANAGEMENT

    }
}