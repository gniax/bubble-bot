using System.Globalization;
using System.Threading;
using BubbleBot.Configurations;
using System.Windows;
using BubbleBot.Configurations.Language;
using System.Reflection;
using System;
using System.IO;
using CefSharp;
using CefSharp.OffScreen;
using System.Runtime.CompilerServices;

namespace BubbleBot.WPF
{
    public partial class App
    {

        public static CultureInfo Culture;
        private static Mutex _mutex;


        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;

            InitializeCefSharp();
            // Singleton application
            _mutex = new Mutex(true, "Bubble Bot", out bool createdNew);
            if (!createdNew)
            {
                Current.Shutdown();
                return;
            }

            GlobalConfiguration.Instance.Load();
            if (!LanguageManager.Initialize())
            {
                MessageBox.Show("Failed to initiliaze langs.");
                Current.Shutdown();
                return;
            }

            BubbleBotMain.Instance.Server.Start();

            Culture = new CultureInfo(GlobalConfiguration.Instance.Lang);
            CultureInfo.CurrentCulture = Culture;
            CultureInfo.CurrentUICulture = Culture;
            CultureInfo.DefaultThreadCurrentCulture = Culture;

            base.OnStartup(e);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            var settings = new CefSettings();

            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                   Environment.Is64BitProcess ? "x64" : "x86",
                                                   "CefSharp.BrowserSubprocess.exe");

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

    }
}
