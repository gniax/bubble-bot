using System;
using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    internal class SubscribeAction : ScriptAction
    {
        // Constructor

        internal override async Task<ScriptActionResults> Process(Account account)
        {
            await account.Network.SendCallAsync(new bakSoftToHardCurrentRateRequestMessage());
            await Task.Delay(3000);
            var subprice = Math.Ceiling(account.Game.bakRate * 800);
            await account.Network.SendCallAsync(new restoreMysteryBoxMessage()).ConfigureAwait(false);
            await account.Network.SendCallAsync(new setShopDetailsRequest()).ConfigureAwait(false);
            await Task.Delay(1500);
            account.Logger.LogInfo(LanguageManager.Translate("606"), LanguageManager.Translate("621"));
            await account.Network.SendCallAsync(new shopOpenRequest()).ConfigureAwait(false);
            await Task.Delay(1500);
            account.Logger.LogInfo(LanguageManager.Translate("606"), LanguageManager.Translate("622"));
            await account.Network.SendCallAsync(new shopOpenCategoryRequest(557, 1, 3)).ConfigureAwait(false);
            await Task.Delay(1500);
            account.Logger.LogInfo(LanguageManager.Translate("606"),
                LanguageManager.Translate("623", subprice.ToString()));
            await account.Network
                .SendCallAsync(new shopBuyRequest("KMS", 800, Convert.ToInt64(subprice), 1, 7537, false))
                .ConfigureAwait(false);
            SpinWait.SpinUntil(() => account.Game.shopBuyInfo != 0, TimeSpan.FromSeconds(30));
            switch (account.Game.shopBuyInfo)
            {
                case 0:
                    account.Game.shopBuyInfo = 0;
                    account.Logger.LogError(LanguageManager.Translate("606"), LanguageManager.Translate("619"));
                    break;

                case 1:
                    // Success
                    var localDate = DateTime.Now;
                    var newDate = localDate.AddDays(7);
                    account.SubscriptionEndDate = newDate;
                    account.Game.shopBuyInfo = 0;
                    account.Logger.LogInfo(LanguageManager.Translate("606"), LanguageManager.Translate("618"));
                    break;
                case -1:
                    account.Game.shopBuyInfo = 0;
                    account.Logger.LogError(LanguageManager.Translate("606"), LanguageManager.Translate("620"));
                    break;
            }

            return ScriptActionResults.DONE;
        }
    }
}