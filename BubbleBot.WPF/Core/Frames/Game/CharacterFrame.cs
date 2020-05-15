using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Game
{
    public static class CharacterFrame
    {
        public static Task HandleEmotePlayMessage(Account account, EmotePlayMessage message)
        {
            return Task.Run(() => account.Game.Character.Update(message));
        }

        public static Task HandleCharacterExperienceGainMessage(Account account, CharacterExperienceGainMessage message)
        {
            return Task.Run(() =>
            {
                account.Statistics.Update(message);

                account.Logger.LogInfo("", LanguageManager.Translate("93", message.ExperienceCharacter));
                account.Game.Character.Stats.Update(message);
            });
        }

        public static Task HandleCharacterStatsListMessage(Account account, CharacterStatsListMessage message)
        {
            return Task.Run(() => account.Game.Character.Update(message));
        }

        public static Task HandleCharacterLevelUpMessage(Account account, CharacterLevelUpMessage message)
        {
            return Task.Run(() =>
            {
                account.Statistics.Update(message);

                account.Logger.LogInfo("", "Level up!");
                account.Game.Character.Update(message);
            });
        }

        public static Task HandleUpdateLifePointsMessage(Account account, UpdateLifePointsMessage message)
        {
            return Task.Run(() => account.Game.Character.Stats.Update(message));
        }

        public static Task HandleGameRolePlayPlayerLifeStatusMessage(Account account,
            GameRolePlayPlayerLifeStatusMessage message)
        {
            return Task.Run(() => account.Game.Character.Update(message));
        }

        public static Task HandlePlayerStatusUpdateMessage(Account account, PlayerStatusUpdateMessage message)
        {
            return Task.Run(() => account.Game.Character.Update(message));
        }

        public static Task HandleLifePointsRegenBeginMessage(Account account, LifePointsRegenBeginMessage message)
        {
            return Task.Run(() => account.Game.Character.Update(message));
        }

        public static Task HandleLifePointsRegenEndMessage(Account account, LifePointsRegenEndMessage message)
        {
            return Task.Run(() =>
            {
                account.Game.Character.Update(message);

                if (message.LifePointsGained > 0)
                    account.Logger.LogInfo("", LanguageManager.Translate("95", message.LifePointsGained));

                account.Game.Character.Stats.Update(message);
            });
        }

        public static Task HandleSpellListMessage(Account account, SpellListMessage message)
        {
            return Task.Run(() => account.Game.Character.UpdateAsync(message));
        }

        public static Task HandleSpellUpgradeSuccessMessage(Account account, SpellUpgradeSuccessMessage message)
        {
            return Task.Run(() => account.Game.Character.Update(message));
        }

        public static Task HandleJobDescriptionMessage(Account account, JobDescriptionMessage message)
        {
            return Task.Run(() => account.Game.Character.Jobs.Update(message));
        }

        public static Task HandleJobExperienceMultiUpdateMessage(Account account,
            JobExperienceMultiUpdateMessage message)
        {
            return Task.Run(async () => await account.Game.Character.Jobs.Update(message));
        }

        public static Task HandleJobExperienceUpdateMessage(Account account, JobExperienceUpdateMessage message)
        {
            return Task.Run(() => account.Game.Character.Jobs.Update(message));
        }

        public static Task HandleMountXpRatioMessage(Account account, MountXpRatioMessage message)
        {
            return Task.Run(() => account.Game.Character.Mount.Update(message));
        }

        public static Task HandleMountRidingMessage(Account account, MountRidingMessage message)
        {
            return Task.Run(() => account.Game.Character.Mount.Update(message));
        }

        public static Task HandleMountSetMessage(Account account, MountSetMessage message)
        {
            return Task.Run(() => account.Game.Character.Mount.Update(message));
        }

        public static Task HandlebakSoftToHardCurrentRateSuccess(Account account,
            bakSoftToHardCurrentRateSuccess message)
        {
            return Task.Run(() =>
            {
                if (message.Rate.HasValue)
                    account.Game.bakRate = (double) message.Rate;
            });
        }

        public static Task HandleshopBuyError(Account account, shopBuyError message)
        {
            return Task.Run(() => account.Game.shopBuyInfo = -1);
        }

        public static Task HandleshopBuySuccess(Account account, shopBuySuccess message)
        {
            return Task.Run(() => account.Game.shopBuyInfo = 1);
        }
    }
}