using System;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using MoonSharp.Interpreter.Debugging;

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
                    int codeCount = account.Scripts.ScriptManager.Script.SourceCodeCount;
                    SourceCode code = account.Scripts.ScriptManager.Script.GetSourceCode(codeCount - 1);

                    if (code.Code.Contains("hasReachFightLimit"))
                    {
                        account.FightLimitReached = true;
                    }
                    else
                    {
                        account.Scripts.StopScript(LanguageManager.Translate("535"));
                    }
                }

            });

        public static Task HandleAccountLoggingKickedMessage(Account account, AccountLoggingKickedMessage message)
            => Task.Run(() =>
            {
                var until = DateTime.Now.AddDays(message.Days).AddHours(message.Hours).AddMinutes(message.Minutes);
                account.Logger.LogError("", LanguageManager.Translate("559", until.ToString("G")));
            });

    }
}
