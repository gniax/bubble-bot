using System.Threading.Tasks;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Game
{
    public static class InventoryFrame
    {
        public static Task HandleKamasUpdateMessage(Account account, KamasUpdateMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleInventoryWeightMessage(Account account, InventoryWeightMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleObjectsQuantityMessage(Account account, ObjectsQuantityMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleObjectQuantityMessage(Account account, ObjectQuantityMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleObjectMovementMessage(Account account, ObjectMovementMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleObjectModifiedMessage(Account account, ObjectModifiedMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleObjectsDeletedMessage(Account account, ObjectsDeletedMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleObjectDeletedMessage(Account account, ObjectDeletedMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleObjectsAddedMessage(Account account, ObjectsAddedMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleObjectAddedMessage(Account account, ObjectAddedMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleInventoryContentMessage(Account account, InventoryContentMessage message)
        {
            return Task.Run(() => account.Game.Character.Inventory.Update(message));
        }

        public static Task HandleDisplayNumericalValueMessage(Account account, DisplayNumericalValueMessage message)
        {
            return Task.Run(() => account.Statistics.Update(message));
        }
    }
}