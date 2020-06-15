using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Groups;
using BubbleBot.Server;
using BubbleBot.Server.Messages;
using BubbleBot.WPF.Views;
using GalaSoft.MvvmLight;
using System.Collections.Specialized;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace BubbleBot
{
    public static class Constants
    {
        public static string WebsiteIpAddress = "http://api.example.com:80"; // Website address
        public static string ApiIpAddress = "http://api.example.com:5001"; // VPS address 
        public static string ServerHost = "api.example.com"; // Server host

        //public static string WebsiteIpAddress = "http://localhost:80"; // Website address
        //public static string ApiIpAddress = "http://localhost:5001"; // VPS address
        //public static string ServerHost = "localhost"; // Server host

        public static int ServerService = 3000; // Server service : 3000
    }

    public class BubbleBotMain : ViewModelBase
    {
        // Fields
        private Account _selectedAccount;

        // Constructor
        public BubbleBotMain()
        {
            Server = new ServerManager();
            Entities = new ObservableCollection<IEntity>();
            // Accounts
            Server.RegisterMessage<LoadAccountMessage>(HandleLoadAccountMessage);
            Server.RegisterMessage<LoadAccountsMessage>(HandleLoadAccountsMessage);
            Server.RegisterMessage<LoadGroupMessage>(HandleLoadGroupMessage);
            Server.RegisterMessage<ConnectAccountMessage>(HandleConnectAccountMessage);
            Server.RegisterMessage<ConnectAccountsMessage>(HandleConnectAccountsMessage);
            Server.RegisterMessage<ConnectGroupMessage>(HandleConnectGroupMessage);
        }

        // Properties
        public ServerManager Server { get; }
        public ObservableCollection<IEntity> Entities { get; }
        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                Set(ref _selectedAccount, value);
            }
        }

        public IEnumerable<Account> ConnectedAccounts => Entities.Select(e =>
        {
            if (e is Account a) return a;
            return (e as Group).Chief;
        });
        public List<Account> EveryConnectedAccount()
        {
            List<Account> res = new List<Account>();
            if (Entities != null && Entities?.Count > 0)
            {
                foreach (var entity in Entities)
                {
                    if (entity is Account acc)
                    {
                        res.Add(acc);
                        continue;
                    }

                    Group group = entity as Group;
                    res.Add(group.Chief);
                    foreach (var member in group.Members)
                    {
                        res.Add(member);
                    }
                }
            }
            return res;
        }

        public void LoadAccounts(IEnumerable<AccountConfiguration> accountConfigs)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                foreach (var accountConfig in accountConfigs)
                    try
                    {
                        var account = new Account(accountConfig);
                        Application.Current.Dispatcher.Invoke(() => Entities.Add(account));
                        SelectedAccount = account;
                    }
                    catch (Exception ex)
                    {
                        await MainWindow.Instance.ShowMessageAsync(LanguageManager.Translate("249"),
                            LanguageManager.Translate("1", accountConfig.Username, ex.Message));
                        Server.SendMessage(new RemoveAccountRequestMessage(accountConfig.Username));
                    }
            });
        }

        public void ConnectAccounts(IEnumerable<AccountConfiguration> accountConfigs)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                foreach (var accountConfig in accountConfigs)
                    try
                    {
                        var account = new Account(accountConfig);
                        Application.Current.Dispatcher.Invoke(() => Entities.Add(account));                  
                        SelectedAccount = account;
                        await Task.Run(account.Connect);
                    }
                    catch (Exception ex)
                    {
                        await MainWindow.Instance.ShowMessageAsync(LanguageManager.Translate("249"),
                            LanguageManager.Translate("1", accountConfig.Username, ex.Message));
                        Server.SendMessage(new RemoveAccountRequestMessage(accountConfig.Username));
                    }
            });
        }

        public void LoadGroup(AccountConfiguration chief, IEnumerable<AccountConfiguration> members)
        {
            var group = new Group(new Account(chief));
            foreach (var member in members) @group.AddMember(new Account(member));

            Application.Current.Dispatcher.Invoke(() =>
            {
                Entities.Add(group);
            });

            SelectedAccount = group.Chief;
        }

        public void ConnectGroup(AccountConfiguration chief, IEnumerable<AccountConfiguration> members)
        {
            var group = new Group(new Account(chief));
            foreach (var member in members) @group.AddMember(new Account(member));

            Application.Current.Dispatcher.Invoke(() =>
            {
                Entities.Add(group);
                SelectedAccount = group.Chief;
                group.Connect();
            });
        }

        public async Task RemoveSelectedAccount()
        {
            if (SelectedAccount == null || _selectedAccount == null)
                return;


            var account = SelectedAccount;
            var index = -1;
            // Remove the account from the list
            for (var i = Entities.Count - 1; i >= 0; i--)
            {
                if (Entities[i] is Account acc && acc == account)
                {
                    index = i;

                    if (index == -1)
                        return;

                    await DisconnectAccount(account);
                    Entities.RemoveAt(i);
                    break;
                }

                if (Entities[i] is Group group)
                {
                    // In case the user wants to remove the chief, remove the whole group
                    if (group.Chief == account)
                    {
                        await RemoveGroup(group, i);
                        return;
                    }

                    for (var j = group.Members.Count - 1; j >= 0; j--)
                    {
                        if (group.Members[j] != account)
                            continue;

                        index = i;
                        await DisconnectAccount(account);
                        group.Members.RemoveAt(j);
                        break;
                    }
                }
            }

            // Set another account as a SelectedAccount
            RefreshSelectedAccount(index);

            // Send the RemoveAccountRequestMessage and dispose the removed account
            Server.SendMessage(new RemoveAccountRequestMessage(account.AccountConfig.Username));
            account?.Dispose();
        }

        private async Task RemoveGroup(Group group, int index)
        {
            // Disconnect the chief
            await DisconnectAccount(group.Chief);

            // Disconnect the members
            for (var i = group.Members.Count - 1; i >= 0; i--) await DisconnectAccount(@group.Members[i]);

            // Remove the group from Entities
            Entities.RemoveAt(index);

            // Set another account as a SelectedAccount
            RefreshSelectedAccount(index);

            // Send the RemoveAccountsRequestMessage and dispose the removed group
            Server.SendMessage(new RemoveAccountsRequestMessage(group.Members.Select(m => m.AccountConfig.Username)
                .Concat(new[] {group.Chief.AccountConfig.Username}).ToList()));
            group.Dispose();
        }

        private void RefreshSelectedAccount(int index)
        {
            // If there are no accounts left, set it to null
            if (Entities.Count == 0 || index == -1)
            {
                SelectedAccount = null;
            }
            // Otherwise look for another one
            else
            {
                index = index > Entities.Count - 1 ? Entities.Count - 1 : index;

                if (Entities[index] is Group group)
                    SelectedAccount = group.Chief;
                else
                    SelectedAccount = Entities[index] as Account;
            }
        }

        public static async Task DisconnectAccount(Account account)
        {
            if (account.Network?.Connected == true)
            {
                await account.Network.Disconnect("CLIENT_CLOSING");
                await Task.Delay(400);
            }
        }

        #region Singleton

        private static BubbleBotMain _instance;

        public static BubbleBotMain Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BubbleBotMain();

                return _instance;
            }
        }

        #endregion

        #region Received messages

        private void HandleLoadAccountMessage(LoadAccountMessage message)
        {
            var accountConfig =
                GlobalConfiguration.Instance.AccountsList.FirstOrDefault(a => a.Username == message.Username);

            if (accountConfig != null) LoadAccounts(new[] {accountConfig});
        }

        private void HandleLoadAccountsMessage(LoadAccountsMessage message)
        {
            var accountsToLoad = new List<AccountConfiguration>();
            foreach (var accountConfig in GlobalConfiguration.Instance.AccountsList)
                if (message.Usernames.Contains(accountConfig.Username))
                    accountsToLoad.Add(accountConfig);

            if (accountsToLoad.Count > 0) LoadAccounts(accountsToLoad);
        }

        private void HandleConnectAccountMessage(ConnectAccountMessage message)
        {
            var accountConfig =
                GlobalConfiguration.Instance.AccountsList.FirstOrDefault(a => a.Username == message.Username);

            if (accountConfig != null) ConnectAccounts(new[] {accountConfig});
        }

        private void HandleConnectAccountsMessage(ConnectAccountsMessage message)
        {
            var accountsToConnect = new List<AccountConfiguration>();
            foreach (var accountConfig in GlobalConfiguration.Instance.AccountsList)
                if (message.Usernames.Contains(accountConfig.Username))
                    accountsToConnect.Add(accountConfig);

            if (accountsToConnect.Count > 0) ConnectAccounts(accountsToConnect);
        }

        private void HandleConnectGroupMessage(ConnectGroupMessage message)
        {
            if (message.Usernames.Count < 2 || message.Usernames.Count > 8)
                return;

            var accountsList = GlobalConfiguration.Instance.AccountsList;
            var chief = accountsList.FirstOrDefault(a => a.Username == message.Usernames[0]);

            if (chief != null)
            {
                var members = new List<AccountConfiguration>();

                for (var i = 0; i < accountsList.Count; i++)
                {
                    if (accountsList[i] == chief)
                        continue;

                    if (message.Usernames.Contains(accountsList[i].Username))
                        members.Add(accountsList[i]);
                }

                // Only connect the group if the numbers fit
                if (message.Usernames.Count == members.Count + 1) ConnectGroup(chief, members);
            }
        }

        private void HandleLoadGroupMessage(LoadGroupMessage message)
        {
            if (message.Usernames.Count < 2 || message.Usernames.Count > 8)
                return;

            var accountsList = GlobalConfiguration.Instance.AccountsList;
            var chief = accountsList.FirstOrDefault(a => a.Username == message.Usernames[0]);

            if (chief != null)
            {
                var members = new List<AccountConfiguration>();

                for (var i = 0; i < accountsList.Count; i++)
                {
                    if (accountsList[i] == chief)
                        continue;

                    if (message.Usernames.Contains(accountsList[i].Username))
                        members.Add(accountsList[i]);
                }

                // Only connect the group if the numbers fit
                if (message.Usernames.Count == members.Count + 1) LoadGroup(chief, members);
            }
        }

        #endregion
    }
}