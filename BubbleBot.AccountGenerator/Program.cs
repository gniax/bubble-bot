using AccountGenerator.Core;
using CefSharp;
using CefSharp.OffScreen;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;



namespace AccountGenerator
{
    class Program
    {
        public static int PROXY_COUNTER = 0; //Start index proxy list
        public static int PROGRAM_MODE = 1; //0-synchrone 1-Asynchrone
        public static int MAX_THREAD = 200;   //si async nombre de compte simultanée
        public static int MAX_TRYBASIC = 5;
        public static int MAX_TRYPROXY = 3;
        public static string ACCOUNT_PASSWORD = "CHANGE_ME";
        public static string ACCOUNT_MAIL = "mailbox";
        public static string API_KEY = "ANTICAPTCHA_API_KEY";

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            InitializeCefSharp();
            string a;
            bool programex = true;

            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("*************************************************************************************");
            Console.WriteLine("*                 ██████╗ ██╗   ██╗██████╗ ██████╗ ██╗     ███████╗                 *");
            Console.WriteLine("*                 ██╔══██╗██║   ██║██╔══██╗██╔══██╗██║     ██╔════╝                 *");
            Console.WriteLine("*                 ██████╔╝██║   ██║██████╔╝██████╔╝██║     █████╗                   *");
            Console.WriteLine("*                 ██╔══██╗██║   ██║██╔══██╗██╔══██╗██║     ██╔══╝                   *");
            Console.WriteLine("*                 ██████╔╝╚██████╔╝██████╔╝██████╔╝███████╗███████╗                 *");
            Console.WriteLine("*                 ╚═════╝  ╚═════╝ ╚═════╝ ╚═════╝ ╚══════╝╚══════╝                 *");
            Console.WriteLine("*                                                                                   *");
            Console.WriteLine("*    ██████╗ ███████╗███╗   ██╗███████╗██████╗  █████╗ ████████╗ ██████╗ ██████╗    *");
            Console.WriteLine("*   ██╔════╝ ██╔════╝████╗  ██║██╔════╝██╔══██╗██╔══██╗╚══██╔══╝██╔═══██╗██╔══██╗   *");
            Console.WriteLine("*   ██║  ███╗█████╗  ██╔██╗ ██║█████╗  ██████╔╝███████║   ██║   ██║   ██║██████╔╝   *");
            Console.WriteLine("*   ██║   ██║██╔══╝  ██║╚██╗██║██╔══╝  ██╔══██╗██╔══██║   ██║   ██║   ██║██╔══██╗   *");
            Console.WriteLine("*   ╚██████╔╝███████╗██║ ╚████║███████╗██║  ██║██║  ██║   ██║   ╚██████╔╝██║  ██║   *");
            Console.WriteLine("*    ╚═════╝ ╚══════╝╚═╝  ╚═══╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝   *");
            Console.WriteLine("*************************************************************************************");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Pour obtenir de l'aide taper le commande: /h");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Programme en attente...");
            while (programex)
            {
                a = Console.ReadLine();

                if (a == "/h")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("-------------------------------------------------------------------------");
                    Console.WriteLine("/s                   :Permet le lancement du programme.");
                    Console.WriteLine("/p                   :Permet de modifier des paramètres du programme.");
                    Console.WriteLine("/e                   :Permet d'arrêter le programme.");
                    Console.WriteLine("/fe                  :Permet de forcer l'arret du programme sans attendre fin de creation du compte en cours.");
                    Console.WriteLine("/h                   :Permet d'affichier l'aide du programme.");
                    Console.WriteLine("-------------------------------------------------------------------------");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (a == "/s")
                {
                    Console.WriteLine("Lancement du programme...");
                    ManagementGeneration debug = new ManagementGeneration();
                    debug.GeneratorManagement().ConfigureAwait(false);
                }
                if (a == "/p")
                {
                    string getP = "";
                    while (true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("************************************************************");
                        Console.WriteLine("Paramètre :");
                        Console.WriteLine("[0]  Proxy configuration");
                        Console.WriteLine("[1]  Account configuration");
                        Console.WriteLine("[2]  Programme specification");
                        Console.WriteLine("[.]  Retour...");
                        Console.WriteLine("************************************************************");
                        Console.ForegroundColor = ConsoleColor.White;
                        getP = Console.ReadLine();
                        if (getP == ".")
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Programme en attente...");
                            break;
                        }
                        if (getP == "0")
                        {
                            while (true)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("************************************************************");
                                Console.WriteLine("Proxy configuration: ");
                                Console.WriteLine("[0]  Index proxy start (Numero du proxy ou commencer la génaration)");
                                Console.WriteLine("[.]  Retour...");
                                Console.WriteLine("************************************************************");
                                Console.ForegroundColor = ConsoleColor.White;
                                getP = Console.ReadLine();
                                if (getP == "0")
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Index proxy start: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    getP = Console.ReadLine();
                                    Int32.TryParse(getP, out PROXY_COUNTER);
                                }
                                if (getP == ".")
                                {
                                    break;
                                }
                            }
                        }
                        if (getP == "1")
                        {
                            while (true)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("************************************************************");
                                Console.WriteLine("Account configuration: ");
                                Console.WriteLine("[0]  Account password");
                                Console.WriteLine("[1]  Account adresse Mail (Exemple: mailbox)");
                                Console.WriteLine("[2]  Anti-captcha key");
                                Console.WriteLine("[.]  Retour...");
                                Console.WriteLine("************************************************************");
                                Console.ForegroundColor = ConsoleColor.White;
                                getP = Console.ReadLine();
                                if (getP == "0")
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Account password: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    getP = Console.ReadLine();
                                    ACCOUNT_PASSWORD = getP;
                                }
                                else if (getP == "1")
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Account mail: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    getP = Console.ReadLine();
                                    ACCOUNT_MAIL = getP;
                                }
                                else if (getP == "2")
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Choisir Anti-captcha key: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    getP = Console.ReadLine();
                                    API_KEY = getP;
                                }
                                else if (getP == ".")
                                {
                                    break;
                                }
                            }
                        }
                        if (getP == "2")
                        {
                            while (true)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("************************************************************");
                                Console.WriteLine("Programme specification: ");
                                Console.WriteLine("[0]  Mode de fonctionement (Simple-thread(defaut):0  Multi-thread:1)");
                                Console.WriteLine("[1]  Nombre maximum de thread (Defaut = 3)");
                                Console.WriteLine("[2]  Nombre maximum de tentative pour chaque action basic (Defaut = 5)");
                                Console.WriteLine("[3]  Nombre maximum de tentative pour la verification du proxy (Defaut = 3)");
                                Console.WriteLine("[.]  Retour...");
                                Console.WriteLine("************************************************************");
                                Console.ForegroundColor = ConsoleColor.White;
                                getP = Console.ReadLine();
                                if (getP == "0")
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Mode de fonctionement: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    getP = Console.ReadLine();
                                    Int32.TryParse(getP, out PROGRAM_MODE);
                                }
                                else if (getP == "1")
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Nombre maximum de thread simultanée: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    getP = Console.ReadLine();
                                    Int32.TryParse(getP, out MAX_THREAD);
                                }
                                else if (getP == "2")
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Nombre de tentative maximum pour la creation de compte: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    getP = Console.ReadLine();
                                    Int32.TryParse(getP, out MAX_TRYBASIC);
                                }
                                else if (getP == "3")
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Nombre de tentative maximum pour la verification du proxy: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    getP = Console.ReadLine();
                                    Int32.TryParse(getP, out MAX_TRYPROXY);
                                }
                                else if (getP == ".")
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                if (a == "/e")
                {
                    Console.WriteLine("Fermeture du programme...");
                    return;
                }
            }

        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            var settings = new CefSettings();
            settings.SetOffScreenRenderingBestPerformanceArgs();

            settings.IgnoreCertificateErrors = true;
            settings.PersistSessionCookies = false;
            settings.PersistUserPreferences = false;
            settings.WindowlessRenderingEnabled = true;

            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                   Environment.Is64BitProcess ? "x64" : "x86",
                                                   "CefSharp.BrowserSubprocess.exe");
            settings.LogSeverity = LogSeverity.Disable;
            settings.CachePath = AppDomain.CurrentDomain.BaseDirectory + "cache";
            settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-shader-disk-cache", "1");
            settings.CefCommandLineArgs.Add("disable-application-cache", "1");
            settings.CefCommandLineArgs.Add("disable-session-storage", "1");
            settings.CefCommandLineArgs.Add("disable-web-security", "0");

            settings.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile/14A403 Safari/602.1";


            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            //settings.CefCommandLineArgs.Add("proxy-server", "http://proxy.example.com:8080");  //"http://proxy.example.com:8811"proxy.example.com:8811 / proxy.example.com:8811 proxy.example.com:8080
            //proxy.example.com:8811 // proxy.example.com:8811 / proxy.example.com:8811 / proxy.example.com:8811

            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
        }

        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }


        private static void myDebug(string texte = "", int numaccount = 0, int numperso = 0, int msgtype = 0)
        {

        }


    }
}