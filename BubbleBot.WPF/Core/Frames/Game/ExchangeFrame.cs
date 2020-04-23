using System.Threading.Tasks;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Game
{
    public static class ExchangeFrame
    {
        public static Task HandleExchangeRequestedTradeMessage(Account account, ExchangeRequestedTradeMessage message)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(200);
                account.Game.Exchange.Update(message);
            });
        }

        public static Task HandleExchangeStartedWithPodsMessage(Account account, ExchangeStartedWithPodsMessage message)
        {
            return Task.Run(() => account.Game.Exchange.Update(message));
        }

        public static Task HandleExchangeObjectAddedMessage(Account account, ExchangeObjectAddedMessage message)
        {
            return Task.Run(() => account.Game.Exchange.Update(message));
        }

        public static Task HandleExchangeObjectModifiedMessage(Account account, ExchangeObjectModifiedMessage message)
        {
            return Task.Run(() => account.Game.Exchange.Update(message));
        }

        public static Task HandleExchangeObjectRemovedMessage(Account account, ExchangeObjectRemovedMessage message)
        {
            return Task.Run(() => account.Game.Exchange.Update(message));
        }

        public static Task HandleExchangeKamaModifiedMessage(Account account, ExchangeKamaModifiedMessage message)
        {
            return Task.Run(() => account.Game.Exchange.Update(message));
        }

        public static Task HandleExchangeIsReadyMessage(Account account, ExchangeIsReadyMessage message)
        {
            return Task.Run(() => account.Game.Exchange.Update(message));
        }

        public static Task HandleExchangeLeaveMessage(Account account, ExchangeLeaveMessage message)
        {
            return Task.Run(() => account.Game.Exchange.Update(message));
        }
    }
}