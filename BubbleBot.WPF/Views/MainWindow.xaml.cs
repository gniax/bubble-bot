using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.Scripts.Managers;
using BubbleBot.Core.Commands;
using BubbleBot.Core.Frames;
using BubbleBot.Core.Groups;
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
using BubbleBot.Data;
using Application = System.Windows.Application;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using MessagesBuilder = BubbleBot.Protocol.Messages.MessagesBuilder;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BubbleBot.WPF.Views
{
    public partial class MainWindow
    {
        // Constructor
        public MainWindow()
        {
            InitializeComponent();

            DataContext = BubbleBotMain.Instance;
            Instance = this;

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Properties.Resources.logo;
            _notifyIcon.DoubleClick += _notifyIcon_DoubleClick;

            BubbleBotMain.Instance.Server.RegisterMessage<FilesHashesMessage>(HandleFilesHashesMessage);
        }
        #region Buttons Event
        private void BtnAccountsManager_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/accountmanager_normal.png";
            BtnAccountsManagerImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void BtnAccountsManager_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/accountmanager_hover.png";
            BtnAccountsManagerImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            this.Cursor = System.Windows.Input.Cursors.Hand;
        }
        private void BtnAccountsManager_Click(object sender, RoutedEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/accountmanager_click.png";
            BtnAccountsManagerImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

            var accountsManagerWindow = new AccountsManagerWindow { Owner = this };
            BubbleBotMain.Instance.AccountsManagerWindow = accountsManagerWindow;
            accountsManagerWindow.ShowDialog();
        }

        private void BtnQuickActions_Click(object sender, RoutedEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/quickaction_click.png";
            BtnQuickActionsImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

            var quickActionsWindow = new QuickActionsWindow { Owner = this };
            quickActionsWindow.ShowDialog();
        }
        private void BtnQuickActions_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/quickaction_normal.png";
            BtnQuickActionsImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void BtnQuickActions_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/quickaction_hover.png";
            BtnQuickActionsImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            this.Cursor = System.Windows.Input.Cursors.Hand;
        }

        private void BtnPlanner_Click(object sender, RoutedEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/planner_click.png";
            BtnPlannerImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

            var plannerWindow = new PlannerWindow { Owner = this };
            plannerWindow.ShowDialog();
        }
        private void BtnPlanner_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/planner_normal.png";
            BtnPlannerImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void BtnPlanner_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/planner_hover.png";
            BtnPlannerImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            this.Cursor = System.Windows.Input.Cursors.Hand;
        }

        private void BtnOptions_Click(object sender, RoutedEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/settings_click.png";
            BtnOptionsImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

            var optionsWindow = new OptionsWindow { Owner = this };
            optionsWindow.ShowDialog();
        }
        private void BtnOptions_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/settings_normal.png";
            BtnOptionsImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void BtnOptions_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/Resources/Buttons/settings_hover.png";
            BtnOptionsImage.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            this.Cursor = System.Windows.Input.Cursors.Hand;
        }

        private void BtnMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            _notifyIcon.Visible = true;
            Hide();
            _notifyIcon.ShowBalloonTip(5000, "Bubble Bot", LanguageManager.Translate("482"), ToolTipIcon.Info);
        }

        #endregion

        // Properties
        public static MainWindow Instance { get; private set; }

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
                    var controller = await this.ShowProgressAsync(LanguageManager.Translate("483"),
                        Randomize.GetRandomLoadingText());
                    await Task.Run(() =>
                    {
                        MessagesBuilder.Initialize();
                        controller.SetProgress(0.14);

                        TypesBuilder.Initialize();
                        controller.SetProgress(0.28);

                        DataManager.Initialize();
                        controller.SetProgress(0.42);

                        MapsManager.Initialize(DTConstants.AssetsVersion);
                        controller.SetProgress(0.56);

                        FramesManager.Initialize();
                        controller.SetProgress(0.70);

                        CommandsHandler.Initialize();

                        BreedsUtility.InitializeAsync().ConfigureAwait(true);
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

        private void BtnConnectGroup_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = (sender as MenuItem).DataContext;
            var group = (Group) datacontext;
            if (group != null) @group.Connect();
        }

        private async void BtnDisconnectGroup_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = (sender as MenuItem).DataContext;
            var group = (Group) datacontext;
            if (group != null) await @group.Disconnect("CLIENT_CLOSING");
        }

        #region Minize/Restore

        private readonly NotifyIcon _notifyIcon;
        private void _notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            Show();
        }

        #endregion
    }
}