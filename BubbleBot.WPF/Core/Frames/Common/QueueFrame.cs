using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Common
{
    public static class QueueFrame
    {
        public static Task HandleQueueStatusMessage(Account account, QueueStatusMessage message)
        {
            return Task.Run(() =>
            {
                account.Logger.LogDofus(LanguageManager.Translate("69"),
                    LanguageManager.Translate("70", message.Position, message.Total));
                account.Network?.ConnectTimeout?.Change(120000, 120000);
            });
        }

        public static Task HandleLoginQueueStatusMessage(Account account, LoginQueueStatusMessage message)
        {
            return Task.Run(() =>
            {
                account.Logger.LogDofus(LanguageManager.Translate("69"),
                    LanguageManager.Translate("70", message.Position, message.Total));
                account.Network?.ConnectTimeout?.Change(120000, 120000);
            });
        }
    }
}