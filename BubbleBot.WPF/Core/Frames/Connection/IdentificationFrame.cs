using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Enums;
using BubbleBot.Protocol.Enums;
using BubbleBot.Protocol.Messages;
using BubbleBot.Utility;
using BubbleBot.Views;

namespace BubbleBot.Core.Frames.Connection
{
    public static class IdentificationFrame
    {
        public static NicknameWindow nicknameWindow;

        public static Task NicknameRegistrationMessage(Account account, NicknameRegistrationMessage message)
        {
            return Task.Run(async () =>
            {
                if (!GlobalConfiguration.Instance.RandomNickname)
                {
                    Application.Current.ExecOnUiThread(() =>
                    {
                        nicknameWindow = new NicknameWindow(account);
                        nicknameWindow.ShowDialog();
                    });

                    while (nicknameWindow == null)
                        Thread.Sleep(2000);

                    while (nicknameWindow.nickname == null)
                        Thread.Sleep(2000);

                    await account.Network.SendMessageAsync(new NicknameChoiceRequestMessage(nicknameWindow.nickname));
                    account.Logger.LogInfo(LanguageManager.Translate("85"),
                        LanguageManager.Translate("615", nicknameWindow.nickname));
                }
                else
                {
                    var randomNickname = Randomize.GetRandomString(16);
                    await account.Network.SendMessageAsync(new NicknameChoiceRequestMessage(randomNickname));
                    account.Logger.LogInfo(LanguageManager.Translate("85"),
                        LanguageManager.Translate("615", randomNickname));
                }
            });
        }

        public static void ExecOnUiThread(this Application app, Action action)
        {
            var dispatcher = app.Dispatcher;
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.BeginInvoke(action);
        }

        public static Task HandleHelloConnectMessage(Account account, HelloConnectMessage message)
        {
            return Task.Run(async () =>
            {
                account.Network.Phase = NetworkPhases.LOGIN;
                account.Logger.LogDebug("IdentificationFrame", LanguageManager.Translate("81"));
                account.FramesData.Key = message.Key.Select(f => (sbyte) f).ToList();
                account.FramesData.Salt = message.Salt;

                await account.Network.SendCallAsync(new LoginMessage(account.FramesData.Salt,
                    account.AccountConfig.Username, account.Token, account.FramesData.Key));
            });
        }

        public static Task HandleNicknameAcceptedMessage(Account account, NicknameAcceptedMessage message)
        {
            return Task.Run(async () => { await account.Network.Disconnect("CLIENT_CLOSING", true); });
        }

        public static Task HandleNicknameRefusedMessage(Account account, NicknameRefusedMessage message)
        {
            return Task.Run(async () =>
            {
                await account.Network.Disconnect("CLIENT_CLOSING", GlobalConfiguration.Instance.AutomaticReconnection);
                account.Logger.LogError(LanguageManager.Translate("85"), LanguageManager.Translate("650"));
            });
        }

        public static Task HandleAssetsVersionCheckedMessage(Account account, AssetsVersionCheckedMessage message)
        {
            Console.WriteLine("HandleAssetsVersionCheckedMessage");
            return Task.Run(async () => await account.Network.SendCallAsync(new LoginMessage(account.FramesData.Salt,
                account.AccountConfig.Username, account.Token, account.FramesData.Key)));
        }

        public static Task HandleConnectionFailedMessage(Account account, ConnectionFailedMessage message)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("HandleConnectionFailedMessage");
                account.Logger.LogError("IdentificationFrame", LanguageManager.Translate("82", message.Reason));
                if (message.Reason != "TIME_OUT" && message.Reason != "KICKED" && message.Reason != "OPT_TIMEOUT")
                {
                    account.PreventAutoReconnection = true;
                    account.PreventPlanificationReconnection = true;
                }
            });
        }

        public static Task HandleIdentificationSuccessMessage(Account account, IdentificationSuccessMessage message)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("HandleIdentificationSuccessMessage");
                account.Login = message.Login;
                account.SubscriptionEndDate = message.SubscriptionEndDate == 0
                    ? null
                    : (DateTime?) DateTime.Now.AddDays(Math.Floor(
                        (message.SubscriptionEndDate - DateTimeOffset.Now.ToUnixTimeMilliseconds()) / 1000 / 60 / 60 /
                        24));

                var log =
                    $"{LanguageManager.Translate("83")}{(message.WasAlreadyConnected ? LanguageManager.Translate("84") : ".")}";
                account.Logger.LogInfo("IdentificationFrame", log);
            });
        }

        public static Task HandleIdentificationFailedMessage(Account account, IdentificationFailedMessage message)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("HandleIdentificationFailedMessage");
                var reason = (IdentificationFailureReasonEnum) message.Reason;
                account.Logger.LogError("IdentificationFrame", LanguageManager.Translate("86", reason));
                if (reason != IdentificationFailureReasonEnum.TIME_OUT &&
                    reason != IdentificationFailureReasonEnum.KICKED &&
                    reason != IdentificationFailureReasonEnum.OTP_TIMEOUT && 
                    reason != IdentificationFailureReasonEnum.WRONG_CREDENTIALS)
                {
                    account.PreventAutoReconnection = true;
                    account.PreventPlanificationReconnection = true;
                }
            });
        }

        public static Task HandleIdentificationFailedBannedMessage(Account account,
            IdentificationFailedBannedMessage message)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("HandleIdentificationFailedBannedMessage");
                account.AccountConfig.IsBan = true;
                GlobalConfiguration.Instance.Save();
                account.State = AccountStates.BANNED;
                var until = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(message.BanEndDate);
                account.Logger.LogError("IdentificationFrame",
                    $"{(IdentificationFailureReasonEnum) message.Reason} [{until.ToShortDateString()} {until.ToShortTimeString()}]");
            });
        }

        public static Task HandleAuthentificationTicketRefusedMessage(Account account,
            AuthenticationTicketRefusedMessage message)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("HandleAuthentificationTicketRefusedMessage");

            });
        }
    }
}