using System;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using MoonSharp.Interpreter.Debugging;
using BubbleBot.Server.Messages;
using BubbleBot.Core.Extensions;
using BubbleBot.Configurations;

namespace BubbleBot.Core.Frames.Common
{
    public static class SecurityFrame
    {

        public static async Task HandleBasicLatencyStatsRequestMessage(Account account, BasicLatencyStatsRequestMessage message)
        {
            await account.Network.SendMessageAsync(new BasicLatencyStatsMessage(262, 12, 50)).ConfigureAwait(false);
        }

        public static async Task HandleSequenceNumberRequestMessage(Account account, SequenceNumberRequestMessage message)
        {
            account.FramesData.Sequence++;
            await account.Network.SendMessageAsync(new SequenceNumberMessage(account.FramesData.Sequence)).ConfigureAwait(false);
        }

        public static Task HandleRecaptchaRequestMessage(Account account, RecaptchaRequestMessage message)
            => Task.Factory.StartNew(async () =>
            {
                account.FramesData.CaptchasCounter++;
                account.Logger.LogWarning(LanguageManager.Translate("71"), LanguageManager.Translate("72", account.FramesData.CaptchasCounter));

                await account.HandleRecaptcha(message.EnrichData.Sitekey);

            }, TaskCreationOptions.LongRunning);

        public static Task HandleTextInformationMessage(Account account, TextInformationMessage message)
            => Task.Run(async () =>
            {
                if (message.MsgId != 245)
                    return;

                if (account.Configuration.DisconnectUponFightsLimit)
                {
                    account.Logger.LogWarning("SecurityFrame", LanguageManager.Translate("534"));
                    
                    if (account.HasGroup && account.IsGroupChief)
                    {
                        await account.Group.Disconnect("CLIENT_CLOSING");
                    }
                    else
                    {
                        await account.Network.Disconnect("CLIENT_CLOSING");
                    }

                }
                else
                {
                    account.FightLimitReached = true;
                }

            });

        public static Task HandleAccountLoggingKickedMessage(Account account, AccountLoggingKickedMessage message)
            => Task.Run(() =>
            {
                account.State = Enums.AccountStates.BANNED;
                account.IsBan = true;
                account.AccountConfig.IsBan = true;
                GlobalConfiguration.Instance.Save();
                var until = DateTime.Now.AddDays(message.Days).AddHours(message.Hours).AddMinutes(message.Minutes);
                BubbleBotMain.Instance.Server.SendMessage(new BotInformationsMessage(
                    account.AccountConfig.Username,
                    account.Game.Character.Level,
                    (byte)account.Game.Character.Stats.EnergyPercent,
                    (byte)account.Game.Character.Inventory.WeightPercent,
                    account.Game.Character.Inventory.Kamas,
                    account.Game.Map.Id,
                    account.Game.Map.CurrentPosition,
                    account.State.ToString(),
                    "-",
                    0,
                    account.Scripts.CurrentScriptName != null ? account.Scripts.CurrentScriptName : "-"
                ));
                account.Logger.LogError("", LanguageManager.Translate("559", until.ToString("G")));
            });

    }
}
