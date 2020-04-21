using System.Threading.Tasks;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Game
{
    public static class StorageFrame
    {
        public static Task HandleExchangeStartedWithStorageMessage(Account account,
            ExchangeStartedWithStorageMessage message)
        {
            return Task.Run(() => account.Game.Storage.Update(message));
        }

        public static Task HandleStorageInventoryContentMessage(Account account, StorageInventoryContentMessage message)
        {
            return Task.Run(() => account.Game.Storage.Update(message));
        }

        public static Task HandleStorageKamasUpdateMessage(Account account, StorageKamasUpdateMessage message)
        {
            return Task.Run(() => account.Game.Storage.Update(message));
        }

        public static Task HandleStorageObjectUpdateMessage(Account account, StorageObjectUpdateMessage message)
        {
            return Task.Run(() => account.Game.Storage.Update(message));
        }

        public static Task HandleStorageObjectRemoveMessage(Account account, StorageObjectRemoveMessage message)
        {
            return Task.Run(() => account.Game.Storage.Update(message));
        }

        public static Task HandleStorageObjectsUpdateMessage(Account account, StorageObjectsUpdateMessage message)
        {
            return Task.Run(() => account.Game.Storage.Update(message));
        }

        public static Task HandleStorageObjectsRemoveMessage(Account account, StorageObjectsRemoveMessage message)
        {
            return Task.Run(() => account.Game.Storage.Update(message));
        }

        public static Task HandleExchangeLeaveMessage(Account account, ExchangeLeaveMessage message)
        {
            return Task.Run(() => account.Game.Storage.Update(message));
        }
    }
}