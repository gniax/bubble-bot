using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Enums;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Game
{
    public static class MapFrame
    {
        // Supposed to be fully connected here
        public static Task HandleCurrentMapMessage(Account account, CurrentMapMessage message)
        {
            return Task.Run(async () =>
            {
                // Notify when a group member or chief is connected
                if (account.Configuration.CreateParty && account.HasGroup && account.PartyId == 0)
                {
                    if (account == account.Group.Chief)
                        account.IsReadyToParty = true;

                    account.Group.PlayerIsOnline?.Invoke(account);
                }

                if (account.Network.ConnectTimeout != null)
                    account.Network.ConnectTimeout.Change(Timeout.Infinite, Timeout.Infinite);

                if (account.State != AccountStates.RECAPTCHA) 
                    account.State = AccountStates.NONE;

                await account.Network.SendMessageAsync(new MapInformationsRequestMessage(message.MapId))
                    .ConfigureAwait(false);
            });
        }

        public static Task HandleMapComplementaryInformationsDataMessage(Account account,
            MapComplementaryInformationsDataMessage message)
        {
            return Task.Factory.StartNew(async () => await account.Game.Map.Update(message).ConfigureAwait(false),
                TaskCreationOptions.LongRunning);
        }

        public static Task HandleMapComplementaryInformationsDataInHouseMessage(Account account,
            MapComplementaryInformationsDataInHouseMessage message)
        {
            return Task.Run(async () =>
                await HandleMapComplementaryInformationsDataMessage(account, message).ConfigureAwait(false));
        }

        public static Task HandleMapComplementaryInformationsWithCoordsMessage(Account account,
            MapComplementaryInformationsWithCoordsMessage message)
        {
            return Task.Run(async () =>
                await HandleMapComplementaryInformationsDataMessage(account, message).ConfigureAwait(false));
        }

        public static Task HandleStatedMapUpdateMessage(Account account, StatedMapUpdateMessage message)
        {
            return Task.Run(() => account.Game.Map.Update(message));
        }

        public static Task HandleInteractiveMapUpdateMessage(Account account, InteractiveMapUpdateMessage message)
        {
            return Task.Run(() => account.Game.Map.Update(message));
        }

        public static Task HandleStatedElementUpdatedMessage(Account account, StatedElementUpdatedMessage message)
        {
            return Task.Run(() => account.Game.Map.Update(message));
        }

        public static Task HandleInteractiveElementUpdatedMessage(Account account,
            InteractiveElementUpdatedMessage message)
        {
            return Task.Run(() => account.Game.Map.Update(message));
        }

        public static Task HandleGameMapMovementMessage(Account account, GameMapMovementMessage message)
        {
            return Task.Run(() =>
            {
                if (account.State == AccountStates.FIGHTING)
                    return;

                account.Game.Map.Update(message);
                account.Game.Managers.Movements.Update(message);
                account.Extensions.CharacterCreation.Update(message);
            });
        }

        public static Task HandleGameContextRemoveElementMessage(Account account,
            GameContextRemoveElementMessage message)
        {
            return Task.Run(() => account.Game.Map.Update(message));
        }

        public static Task HandleTeleportOnSameMapmessage(Account account, TeleportOnSameMapMessage message)
        {
            return Task.Run(() =>
                account.Game.Map.Players.FirstOrDefault(p => p.Id == message.TargetId)?.Update(message));
        }

        public static Task HandleGameContextRemoveMultipleElementsMessage(Account account,
            GameContextRemoveMultipleElementsMessage message)
        {
            return Task.Run(() => account.Game.Map.Update(message));
        }

        public static Task HandleGameRolePlayShowActorMessage(Account account, GameRolePlayShowActorMessage message)
        {
            return Task.Run(() => account.Game.Map.Update(message));
        }

        public static Task HandleGameMapNoMovementMessage(Account account, GameMapNoMovementMessage message)
        {
            return Task.Run(async () =>
            {
                if (account.State == AccountStates.FIGHTING || account.State == AccountStates.RECAPTCHA)
                    return;

                account.State = AccountStates.NONE;
                await account.Game.Managers.Movements.Update(message);
            });
        }
    }
}