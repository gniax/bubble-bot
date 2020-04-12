using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.Scripts.Managers;
using BubbleBot.Core.Commands;
using BubbleBot.Core.Frames;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Data.Maps;
using BubbleBot.Protocol.Types;
using BubbleBot.Server.Messages;
using BubbleBot.Updates;
using BubbleBot.Utility;
using BubbleBot.Utility.DofusTouch;
using BubbleBot.Views;
using BubbleBot.Views.Planner;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace BubbleBot.WPF.Views
{
    public partial class MainWindow
    {

        // Properties
        public static MainWindow Instance { get; private set; }


        // Constructor
        public MainWindow()
        {
            InitializeComponent();

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Properties.Resources.logo;
            _notifyIcon.DoubleClick += _notifyIcon_DoubleClick;

            DataContext = BubbleBotMain.Instance;
            Instance = this;

            BubbleBotMain.Instance.Server.RegisterMessage<FilesHashesMessage>(HandleFilesHashesMessage);
        }


        private void HandleFilesHashesMessage(FilesHashesMessage message)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                // Updater
                if (await this.ShowUpdatesAsync(message.FilesHashes))
                {
                    Environment.Exit(0);
                    return;
                }

                // Loading
                try
                {
                    var controller = await this.ShowProgressAsync(LanguageManager.Translate("483"), Randomize.GetRandomLoadingText());
                    await Task.Run(async () =>
                    {
                        Protocol.Messages.MessagesBuilder.Initialize();
                        controller.SetProgress(0.14);

                        TypesBuilder.Initialize();
                        controller.SetProgress(0.28);

                        DataManager.Initialize(DTConstants.AssetsVersion, GlobalConfiguration.Instance.Lang);
                        controller.SetProgress(0.42);

                        MapsManager.Initialize(DTConstants.AssetsVersion);
                        controller.SetProgress(0.56);

                        FramesManager.Initialize();
                        controller.SetProgress(0.70);

                        CommandsHandler.Initialize();

                        BreedsUtility.Initialize();
                        controller.SetProgress(1);

                        LuaScriptManager.Initialize();
                    });

                    await controller.CloseAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });
        }

        private void MetroWindow_ContentRendered(object sender, EventArgs e)
        {
            BubbleBotMain.Instance.Server.SendMessage(new FilesHashesRequestMessage());
        }

        private void BtnAccountsManager_Click(object sender, RoutedEventArgs e)
        {
            var accountsManagerWindow = new AccountsManagerWindow { Owner = this };
            accountsManagerWindow.ShowDialog();
        }

        private void BtnOptions_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow { Owner = this };
            optionsWindow.ShowDialog();
        }

        private void BtnQuickActions_Click(object sender, RoutedEventArgs e)
        {
            var quickActionsWindow = new QuickActionsWindow { Owner = this };
            quickActionsWindow.ShowDialog();
        }

        private void BtnPlanner_Click(object sender, RoutedEventArgs e)
        {
            var plannerWindow = new PlannerWindow { Owner = this };
            plannerWindow.ShowDialog();
        }

        #region Minize/Restore

        private readonly NotifyIcon _notifyIcon;

        private void BtnMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            _notifyIcon.Visible = true;
            Hide();
            _notifyIcon.ShowBalloonTip(5000, "Bubble Bot", LanguageManager.Translate("482"), ToolTipIcon.Info);
        }

        private void _notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            Show();
        }

        #endregion

    }
}
