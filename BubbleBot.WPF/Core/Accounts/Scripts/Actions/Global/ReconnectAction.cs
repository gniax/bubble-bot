using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class ReconnectAction : ScriptAction
    {
        // Constructor
        public ReconnectAction(int s, bool rs)
        {
            Seconds = s;
            RestartScript = rs;
        }

        // Properties
        public int Seconds { get; }

        public bool RestartScript { get; }


        // Reconnection Function
        // Exception : reconnect(0, true||false) leads to an instant reconnection
        internal override async Task<ScriptActionResults> Process(Account account)
        {
            var localDate = DateTime.Now;
            var newDate = localDate.AddSeconds(Seconds);

            var newDateToDay =
                newDate.Day + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(newDate.Month);
            var newDateToTime = newDate.ToString("HH:mm:ss");

            account.Logger.LogMessage(LanguageManager.Translate("165"),
                LanguageManager.Translate("612", newDateToDay, newDateToTime, RestartScript.ToString()));
            account.PreventPlanificationReconnection = true;
            await account.Network.Disconnect("CLIENT_CLOSING");
            await Task.Delay(400);

            // Note: Here we'll log informations about current timer before reconnection
            // More the longer the time, and more will be display informations about situation
            // Ex: For 30 seconds reconnection -> 1 display at the half (show 15 seconds remaining)
            // For 86440s (1 day) -> a display each hour
            for (var i = 0; i < Seconds; i++)
            {
                var factor = 0;

                if (Seconds > 30 && Seconds <= 300)
                    factor = 2;
                else if (Seconds > 300 && Seconds <= 1800)
                    factor = 3;
                else if (Seconds > 1800 && Seconds <= 7200)
                    factor = 5;
                else if (Seconds > 7200 && Seconds <= 43200)
                    factor = 8;
                else
                    factor = 12;

                if (i == Seconds - 60 || Enumerable.Range(1, factor - 1).Any(n => i == Seconds / factor * n))
                {
                    var time = TimeSpan.FromSeconds(Seconds - i);

                    var format = @"hh\:mm\:ss";

                    if (Seconds - i < 60)
                        format = @"ss";
                    else if (Seconds - i < 3600)
                        format = @"mm\:ss";

                    var timeDisplay = time.ToString(format);
                    account.Logger.LogMessage(LanguageManager.Translate("165"),
                        LanguageManager.Translate("614", timeDisplay));
                }

                // Here set the delay to 1sec
                // Note: We get for 22 hours delay, 22h20 real delay 
                // it means the 1s function delay is longer than the right 1 sec -> we needs a coefficient to settle the timer 
                // 22h20 = 80400s && 22h = 79200s => 80400/79200 ~= 1.015 // 1000 / 1.015 ~= 985 
                // TODO -- check if this theorical calculation is suitable
                // update: there's a +4 secs offset every 1h20 => unable to fix it => 984 is not enough
                await Task.Delay(985);
            }

            if (account.Network.Connected)
                return ScriptActionResults.FAILED;

            await account.Connect().ConfigureAwait(true);

            await Task.Delay(3000);

            if (account.Network.Connected && RestartScript)
            {
                account.WaitForRestartScript = true;
                return ScriptActionResults.DONE;
            }

            if (account.Network.Connected && !RestartScript)
                return ScriptActionResults.DONE;
            if (!account.Network.Connected) return ScriptActionResults.FAILED;

            return ScriptActionResults.FAILED;
        }
    }
}