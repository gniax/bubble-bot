using System;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Enums;
using BubbleBot.Protocol.Messages;
using BubbleBot.Utility.DofusTouch;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using System.Net.WebSockets;
using BubbleBot.Views;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using BubbleBot.Configurations;
using BubbleBot.Utility;

namespace BubbleBot.Core.Frames.Connection
{
    public static class IdentificationFrame
    {
        public static NicknameWindow nicknameWindow;
        public static Task NicknameRegistrationMessage(Account account, NicknameRegistrationMessage message)
            => Task.Run(async () =>
            {
                if (!GlobalConfiguration.Instance.RandomNickname)
                {
                    Application.Current.ExecOnUiThread(() =>
                    {
                        nicknameWindow = new NicknameWindow(account);
                        nicknameWindow.ShowDialog();
                    });

                    while (nicknameWindow == null)
                        System.Threading.Thread.Sleep(2000);

                    while (nicknameWindow.nickname == null)
                        System.Threading.Thread.Sleep(2000);

                    await account.Network.SendMessageAsync(new NicknameChoiceRequestMessage(nicknameWindow.nickname));
                    account.Logger.LogInfo(LanguageManager.Translate("85"), LanguageManager.Translate("615", nicknameWindow.nickname));
                }
                else
                {
                    string randomNickname = BubbleBot.Utility.Randomize.GetRandomString(16);
                    await account.Network.SendMessageAsync(new NicknameChoiceRequestMessage(randomNickname));
                    account.Logger.LogInfo(LanguageManager.Translate("85"), LanguageManager.Translate("615", randomNickname));
                }

            });
        public static void ExecOnUiThread(this Application app, Action action)
        {
            var dispatcher = app.Dispatcher;
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.BeginInvoke(action);
        }
        public static Task HandleHelloConnectMessage(Account account, HelloConnectMessage message)
            => Task.Run(async () =>
            {
                account.Network.Phase = Enums.NetworkPhases.LOGIN;
                account.Logger.LogDebug("IdentificationFrame", LanguageManager.Translate("81"));
                account.FramesData.Key = message.Key.Select(f => (sbyte)f).ToList();
                account.FramesData.Salt = message.Salt;

                await account.Network.SendCallAsync(new LoginMessage(account.FramesData.Salt, account.AccountConfig.Username, account.Token, account.FramesData.Key));
                //await account.Network.SendCallAsync(new CheckAssetsVersionMessage(DTConstants.AssetsVersion, DTConstants.StaticDataVersion));
                //string data = "4{\"call\":\"login\",\"data\":{\"username\":\"" + account.AccountConfig.Username + "\",\"token\":\"" + account.Token + "\",\"salt\":\"" + account.FramesData.Salt + "\",\"key\":[" + account.FramesData.Key + "]}}";
                //await account.Network.SendRawAsync(data);
            });

        public static Task HandleNicknameAcceptedMessage(Account account, NicknameAcceptedMessage message)
            => Task.Run(async () =>
            {
                await account.Network.Disconnect("CLIENT_CLOSING", true);
            });

        public static Task HandleAssetsVersionCheckedMessage(Account account, AssetsVersionCheckedMessage message)
        {
            Console.WriteLine("HandleAssetsVersionCheckedMessage");
            return Task.Run(async () => await account.Network.SendCallAsync(new LoginMessage(account.FramesData.Salt, account.AccountConfig.Username, account.Token, account.FramesData.Key)));
        }
        public static Task HandleConnectionFailedMessage(Account account, ConnectionFailedMessage message)
        {
            Console.WriteLine("HandleConnectionFailedMessage");
            return Task.Run(() => account.Logger.LogError("IdentificationFrame", LanguageManager.Translate("82", message.Reason)));
        }

        public static Task HandleIdentificationSuccessMessage(Account account, IdentificationSuccessMessage message)
            => Task.Run(() =>
            {
                Console.WriteLine("HandleIdentificationSuccessMessage");
                account.Login = message.Login;
                account.SubscriptionEndDate = message.SubscriptionEndDate == 0 ? 
                                              null : 
                                              (DateTime?)DateTime.Now.AddDays(Math.Floor((message.SubscriptionEndDate - DateTimeOffset.Now.ToUnixTimeMilliseconds()) / 1000 / 60 / 60 / 24));

                var log = $"{LanguageManager.Translate("83")}{(message.WasAlreadyConnected ? LanguageManager.Translate("84") : ".")}";
                account.Logger.LogInfo("IdentificationFrame", log);
            });

        public static Task HandleIdentificationFailedMessage(Account account, IdentificationFailedMessage message)
            => Task.Run(() =>
            {
                Console.WriteLine("HandleIdentificationFailedMessage");
                IdentificationFailureReasonEnum reason = (IdentificationFailureReasonEnum)message.Reason;
                account.Logger.LogError("IdentificationFrame", LanguageManager.Translate("86", reason));
            });

        public static Task HandleIdentificationFailedBannedMessage(Account account, IdentificationFailedBannedMessage message)
            => Task.Run(() =>
            {
                Console.WriteLine("HandleIdentificationFailedBannedMessage");
                account.IsBan = true;
                account.State = Enums.AccountStates.BANNED;
                DateTime until = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(message.BanEndDate);
                account.Logger.LogError("IdentificationFrame", $"{(IdentificationFailureReasonEnum)message.Reason} [{until.ToShortDateString()} {until.ToShortTimeString()}]");
            });

    }
}
