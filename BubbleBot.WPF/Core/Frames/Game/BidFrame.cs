using System.Threading.Tasks;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Game
{
    public static class BidFrame
    {
        public static Task HandleExchangeStartedBidBuyerMessage(Account account, ExchangeStartedBidBuyerMessage message)
        {
            return Task.Run(() => account.Game.Bid.Update(message));
        }

        public static Task HandleExchangeTypesItemsExchangerDescriptionForUserMessage(Account account,
            ExchangeTypesItemsExchangerDescriptionForUserMessage message)
        {
            return Task.Run(() => account.Game.Bid.Update(message));
        }

        public static Task HandleExchangeStartedBidSellerMessage(Account account,
            ExchangeStartedBidSellerMessage message)
        {
            return Task.Run(() => account.Game.Bid.Update(message));
        }

        public static Task HandleExchangeBidPriceMessage(Account account, ExchangeBidPriceMessage message)
        {
            return Task.Run(() => account.Game.Bid.Update(message));
        }

        public static Task HandleExchangeErrorMessage(Account account, ExchangeErrorMessage message)
        {
            return Task.Run(() => account.Game.Bid.Update(message));
        }

        public static Task HandleExchangeLeaveMessage(Account account, ExchangeLeaveMessage message)
        {
            return Task.Run(() => account.Game.Bid.Update(message));
        }
    }
}