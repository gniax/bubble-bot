using System;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Enums;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Game
{
    public static class FightFrame
    {
        public static Task HandleGameMapMovementMessage(Account account, GameMapMovementMessage message)
        {
            return Task.Run(() =>
            {
                if (account.State == AccountStates.FIGHTING)
                {
                    account.Game.Fight.Update(message);
                    account.Extensions.CharacterCreation.Update(message);
                }
            });
        }

        public static Task HandleGameMapNoMovementMessage(Account account, GameMapNoMovementMessage message)
        {
            return Task.Run(() =>
            {
                if (account.State != AccountStates.FIGHTING)
                    return;

                account.Logger.LogError(LanguageManager.Translate("472"), LanguageManager.Translate("55"));
                //await account.Extensions.Fights.Update(message);

                // In case we get into a fight after we "tried" to move
                account.Game.Managers.Movements.Clear();
            });
        }

        public static Task HandleGameActionFightNoSpellCastMessage(Account account,
            GameActionFightNoSpellCastMessage message)
        {
            return Task.Run(async () => { await account.Extensions.Fights.Update(message); });
        }

        public static Task HandleGameFightPlacementPossiblePositionsMessage(Account account,
            GameFightPlacementPossiblePositionsMessage message)
        {
            return Task.Factory.StartNew(async () =>
            {
                account.Game.Fight.Update(message);
                await account.Extensions.Fights.Update(message);
            }, TaskCreationOptions.LongRunning);
        }

        public static Task HandleSequenceEndMessage(Account account, SequenceEndMessage message)
        {
            return Task.Run(async () =>
            {
                if (account.Game.Character.Id == message.AuthorId)
                {
                    await Task.Delay(200 * (int) account.Extensions.Fights.Configuration.FightsSpeed);
                    await account.Network.SendMessageAsync(
                        new GameActionAcknowledgementMessage(true, (int) message.ActionId));
                }

                await account.Extensions.Fights.Update(message);
            });
        }

        public static Task HandleGameFightTurnReadyRequestMessage(Account account,
            GameFightTurnReadyRequestMessage message)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(message.Id == account.Game.Character.Id
                    ? 200
                    : 400 * (int) account.Extensions.Fights.Configuration.FightsSpeed);
                await account.Network.SendMessageAsync(new GameFightTurnReadyMessage(true));
            });
        }

        public static Task HandleGameFightShowFighterRandomStaticPoseMessage(Account account,
            GameFightShowFighterRandomStaticPoseMessage message)
        {
            return HandleGameFightShowFighterMessage(account, message);
        }

        public static Task HandleGameFightNewRoundMessage(Account account, GameFightNewRoundMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameActionFightSpellCastMessage(Account account,
            GameActionFightSpellCastMessage message)
        {
            return Task.Run(() =>
            {
                account.Game.Fight.UpdateAsync(message).ConfigureAwait(false);
                account.Extensions.CharacterCreation.Update(message);
                //account.Logger.LogFight(account.Game.Fight.GetFighter(message.SourceId).GetName(), $"a lancé {DataManager.Get<Spells>((int)message.SpellId).NameId}.");
            });
        }

        public static Task HandleGameFightEndMessage(Account account, GameFightEndMessage message)
        {
            return Task.Run(() =>
            {
                var elapsedTime = TimeSpan.FromMilliseconds(message.Duration);
                account.Logger.LogFight(LanguageManager.Translate("101"),
                    LanguageManager.Translate("97", elapsedTime.ToString(@"mm\m\:ss\s")));

                account.Game.Fight.Update(message);
                account.Statistics.Update(message);
            });
        }

        public static Task HandleGameActionFightDispellableEffectMessage(Account account,
            GameActionFightDispellableEffectMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameFightTurnEndMessage(Account account, GameFightTurnEndMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameFightTurnStartMessage(Account account, GameFightTurnStartMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameFightLeaveMessage(Account account, GameFightLeaveMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameActionFightLifePointsGainMessage(Account account,
            GameActionFightLifePointsGainMessage message)
        {
            return Task.Run(() =>
            {
                account.Game.Fight.Update(message);

                var fighter = account.Game.Fight.GetFighter(message.TargetId);
                account.Logger.LogFight(fighter.GetName(), $"+ {message.Delta} HP.");
            });
        }

        public static Task HandleGameActionFightLifePointsLostMessage(Account account,
            GameActionFightLifePointsLostMessage message)
        {
            return Task.Run(() =>
            {
                account.Game.Fight.Update(message);

                var fighter = account.Game.Fight.GetFighter(message.TargetId);
                if (fighter != null)
                    account.Logger.LogFight(fighter.GetName(),
                        $"- {message.Loss} HP{(fighter.LifePoints == 0 ? " (mort)." : ".")}");
            });
        }

        public static Task HandleGameActionFightSummonMessage(Account account, GameActionFightSummonMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameActionFightSlideMessage(Account account, GameActionFightSlideMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameActionFightTeleportOnSameMapMessage(Account account,
            GameActionFightTeleportOnSameMapMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameActionFightDeathMessage(Account account, GameActionFightDeathMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameActionFightPointsVariationMessage(Account account,
            GameActionFightPointsVariationMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameActionFightCloseCombatMessage(Account account, GameActionFightCloseCombatMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleFighterStatsListMessage(Account account, FighterStatsListMessage message)
        {
            return Task.Run(async () =>
            {
                account.Game.Fight.Update(message);
                await account.Network.SendMessageAsync(new GameActionAcknowledgementMessage {Valid = true});
            });
        }

        public static Task HandleGameFightSynchronizeMessage(Account account, GameFightSynchronizeMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameEntitiesDispositionMessage(Account account, GameEntitiesDispositionMessage message)
        {
            return Task.Run(async () =>
            {
                account.Game.Fight.Update(message);
                await account.Extensions.CharacterCreation.Update(message);
            });
        }

        public static Task HandleGameActionFightInvisibilityMessage(Account account, GameActionFightInvisibilityMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameActionFightInvisibleDetectedMessage(Account account, GameActionFightInvisibleDetectedMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameFightOptionStateUpdateMessage(Account account,
            GameFightOptionStateUpdateMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameFightUpdateTeamMessage(Account account, GameFightUpdateTeamMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameFightShowFighterMessage(Account account, GameFightShowFighterMessage message)
        {
            return Task.Run(async () =>
            {
                account.Game.Fight.Update(message);
                await account.Extensions.Fights.Update(message);
            });
        }

        public static Task HandleTextInformationMessage(Account account, TextInformationMessage message)
        {
            return Task.Run(() => account.Game.Fight.Update(message));
        }

        public static Task HandleGameFightStartMessage(Account account, GameFightStartMessage message)
        {
            return Task.Run(() =>
            {
                account.Logger.LogFight(LanguageManager.Translate("101"), LanguageManager.Translate("98"));
                account.Game.Fight.Update(message);
            });
        }

        public static Task HandleGameFightStartingMessage(Account account, GameFightStartingMessage message)
        {
            return Task.Run(() => account.Extensions.CharacterCreation.Update(message));
        }

        public static Task HandleGameFightJoinMessage(Account account, GameFightJoinMessage message)
        {
            return Task.Run(() =>
            {
                account.Game.Map.Update(message);
                account.Logger.LogFight(LanguageManager.Translate("101"), LanguageManager.Translate("99"));
                account.Game.Fight.Update(message);
            });
        }
    }
}