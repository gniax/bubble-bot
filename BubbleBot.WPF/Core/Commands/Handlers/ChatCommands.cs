using System.Threading.Tasks;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Enums;

namespace BubbleBot.Core.Commands.Handlers
{
    public class ChatCommands
    {
        [Command("g")]
        public static async Task SendToGuildCommand(Account account, [Remainer] string message)
        {
            await account.Game.Chat.SendMessage(message, ChatActivableChannelsEnum.CHANNEL_GUILD);
        }

        [Command("w")]
        public static async Task SendWhisperToCommand(Account account, string receiver, [Remainer] string message)
        {
            await account.Game.Chat.SendMessageTo(message, receiver);
        }

        [Command("p")]
        public static async Task SendToPartyCommand(Account account, [Remainer] string message)
        {
            await account.Game.Chat.SendMessage(message, ChatActivableChannelsEnum.CHANNEL_PARTY);
        }

        [Command("r")]
        public static async Task SendToSeekCommand(Account account, [Remainer] string message)
        {
            await account.Game.Chat.SendMessage(message, ChatActivableChannelsEnum.CHANNEL_SEEK);
        }

        [Command("b")]
        public static async Task SendToSalesCommand(Account account, [Remainer] string message)
        {
            await account.Game.Chat.SendMessage(message, ChatActivableChannelsEnum.CHANNEL_SALES);
        }

        [Command("i")]
        public static async Task SendToNoobCommand(Account account, [Remainer] string message)
        {
            await account.Game.Chat.SendMessage(message, ChatActivableChannelsEnum.CHANNEL_NOOB);
        }
    }
}