using BubbleBot.Configurations.Language;
using BubbleBot.Core.Enums;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class ReconnectAction : ScriptAction
    {

        // Properties
        public int Seconds { get; private set; }

        public bool RestartScript { get; private set; }

        // Constructor
        public ReconnectAction(int s, bool rs)
        {
            Seconds = s;
            RestartScript = rs;
        }


        // Reconnection Function
        // Exception : reconnect(0, true||false) leads to an instant reconnection
        internal override async Task<ScriptActionResults> Process(Account account)
        {
            DateTime localDate = DateTime.Now;
            DateTime newDate = localDate.AddSeconds(Seconds);

            string newDateToDay = newDate.Day.ToString() + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(newDate.Month);
            string newDateToTime= newDate.ToString("HH:mm:ss");

            account.Logger.LogMessage(LanguageManager.Translate("165"), LanguageManager.Translate("612", newDateToDay, newDateToTime, RestartScript.ToString()));
            await account.Network.Disconnect("CLIENT_CLOSING");
            await Task.Delay(400);
            
            // Note: Here we'll log informations about current timer before reconnection
            // More the longer the time, and more will be display informations about situation
            // Ex: For 30 seconds reconnection -> 1 display at the half (show 15 seconds remaining)
            // For 86440s (1 day) -> a display each hour
            for(int i=0; i<Seconds; i++)
            {
                int factor = 0;

                if (Seconds > 30 && Seconds <= 300)
                {
                    factor = 2;
                }
                else if (Seconds > 300 && Seconds <= 1800)
                {
                    factor = 3;
                }
                else if (Seconds > 1800 && Seconds <= 7200)
                {
                    factor = 5;
                }
                else if (Seconds > 7200 && Seconds <= 43200)
                {
                    factor = 8;
                }
                else
                {
                    factor = 12;
                }

                if (i == Seconds - 60 || Enumerable.Range(1, factor - 1).Any(n => i == (Seconds / factor * n)))
                {
                    TimeSpan time = TimeSpan.FromSeconds(Seconds - i);

                    string format = @"hh\:mm\:ss";

                    if ((Seconds - i) < 60)
                        format = @"ss";
                    else if ((Seconds - i) < 3600)
                        format = @"mm\:ss";

                    string timeDisplay = time.ToString(format);
                    account.Logger.LogMessage(LanguageManager.Translate("165"), LanguageManager.Translate("614", timeDisplay));
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

            await account.Connect();

            account.Logger.LogMessage(LanguageManager.Translate("12"), LanguageManager.Translate("616", 20));
            await Task.Delay(20000);
            if (account.Network.Phase == NetworkPhases.GAME)
            {
                if (RestartScript == true)
                {
                    if (account.HasGroup && account.IsGroupChief)
                    {
                        account.Group.Chief.Scripts.StartScript();
                    }
                    else if(!account.HasGroup)
                    {
                        SpinWait.SpinUntil(() => (!account.IsBusy), TimeSpan.FromSeconds(10));
                        await Task.Delay(1500);
                        account.Scripts.StartScript();

                    }
                }
                        
                return ScriptActionResults.DONE;
            }
            else // If the reconnection failed ? server busy ? bann ? => Retry
            {
                await account.Connect();
                account.Logger.LogMessage(LanguageManager.Translate("12"), LanguageManager.Translate("616", 20));
                await Task.Delay(20000);
                if (account.Network.Connected)
                {
                    if (RestartScript == true)
                    {
                        if (account.HasGroup && account.IsGroupChief)
                        {
                            account.Group.Chief.Scripts.StartScript();
                        }
                        else if (!account.HasGroup)
                        {
                            SpinWait.SpinUntil(() => (!account.IsBusy), TimeSpan.FromSeconds(10));
                            await Task.Delay(1500);
                            account.Scripts.StartScript();

                        }
                    }

                    return ScriptActionResults.DONE;
                }

                // If it takes a long time to reconnect, we will try a second time
                if (Seconds > 1800)
                {
                    account.Logger.LogMessage(LanguageManager.Translate("617"), LanguageManager.Translate("613"));
                    await Task.Delay(300 * 1000);
                    await account.Connect();
                    if (account.Network.Connected)
                    {
                        if (RestartScript == true)
                        {
                            if (account.HasGroup && account.IsGroupChief)
                            {
                                account.Group.Chief.Scripts.StartScript();
                            }
                            else if (!account.HasGroup)
                            {
                                SpinWait.SpinUntil(() => (!account.IsBusy), TimeSpan.FromSeconds(10));
                                await Task.Delay(1500);
                                account.Scripts.StartScript();

                            }
                        }

                        return ScriptActionResults.DONE;
                    }
                }

                account.Network.Disconnect("CLIENT_CLOSING", true);
                return ScriptActionResults.FAILED;
            }
        }

    }
}
