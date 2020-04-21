using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using CefSharp;
using CefSharp.OffScreen;

#pragma warning disable MSB3270

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
            _mutex = new Mutex(true, "Bubble Bot", out var createdNew);
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

        protected override void OnExit(ExitEventArgs e)
        {
            BubbleBotMain.Instance.Cleanup();
            GlobalConfiguration.Instance.Cleanup();
            Cef.Shutdown();
            base.OnExit(e);
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

            settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-shader-disk-cache", "1");

            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            Cef.Initialize(settings, false, browserProcessHandler: null);
        }

        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                var assemblyName = args.Name.Split(new[] {','}, 2)[0] + ".dll";
                var archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
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