using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Messages;
using BubbleBot.Data;

namespace BubbleBot.Core.Frames.Game
{
    public static class AchievementsFrame
    {
        public static Task HandleAchievementRewardSuccessMessage(Account account,
            AchievementRewardSuccessMessage message)
        {
            return Task.Run(() =>
            {
                account.Statistics.Update(message);

                var achievement = DataManager.Get<Achievements>(message.AchievementId);
                account.Logger.LogInfo("", LanguageManager.Translate("92", achievement.NameId, achievement.Points));
            });
        }

        public static Task HandleAchievementFinishedMessage(Account account, AchievementFinishedMessage message)
        {
            return Task.Run(async () =>
            {
                if (!account.Configuration.AcceptAchievements)
                    return;

                await account.Network.SendMessageAsync(new AchievementRewardRequestMessage((int) message.Id));
            });
        }

        public static Task HandleAchievementListMessage(Account account, AchievementListMessage message)
        {
            return Task.Run(async () =>
            {
                if (!account.Configuration.AcceptAchievements)
                    return;

                foreach (var achiv in message.RewardableAchievements)
                    await account.Network.SendMessageAsync(new AchievementRewardRequestMessage((int) achiv.Id));
            });
        }
    }
}