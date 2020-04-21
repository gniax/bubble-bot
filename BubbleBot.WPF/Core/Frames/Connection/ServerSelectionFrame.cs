using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Enums;
using BubbleBot.Protocol.Enums;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Connection
{
    public static class ServerSelectionFrame
    {
        public static Task HandleServersListMessage(Account account, ServersListMessage message)
        {
            return Task.Run(async () =>
            {
                var server = account.AccountConfig.CharacterCreation.Create
                    ? message.Servers.FirstOrDefault(s => s.Name == account.AccountConfig.CharacterCreation.Server)
                    : account.AccountConfig.Server == "-"
                        ? message.Servers.FirstOrDefault(s => s.CharactersCount > 0)
                        : message.Servers.FirstOrDefault(s => s.Name == account.AccountConfig.Server);

                if (server == null || server.CharactersCount == 0 && !account.AccountConfig.CharacterCreation.Create)
                {
                    account.Logger.LogError(LanguageManager.Translate("87"),
                        LanguageManager.Translate("88", account.AccountConfig.Server));
                    return;
                }

                var status = (ServerStatusEnum) server.Status;
                if (status != ServerStatusEnum.ONLINE && status != ServerStatusEnum.SAVING && !server.IsSelectable)
                {
                    account.Logger.LogError(LanguageManager.Translate("87"),
                        LanguageManager.Translate("89", server.Name, status));
                    return;
                }

                if (status == ServerStatusEnum.SAVING)
                {
                    account.FramesData.ServerToAutoConnectTo = server.Id;
                    account.Logger.LogInfo(LanguageManager.Translate("87"),
                        LanguageManager.Translate("573", server.Name));
                    account.Network?.ConnectTimeout?.Change(Timeout.Infinite, Timeout.Infinite);
                }
                else // ONLINE
                {
                    account.Logger.LogDebug(LanguageManager.Translate("87"),
                        LanguageManager.Translate("90", server.Name));
                    await account.Network.SendMessageAsync(new ServerSelectionMessage((int) server.Id));
                }
            });
        }

        public static Task HandleServerStatusUpdateMessage(Account account, ServerStatusUpdateMessage message)
        {
            return Task.Run(async () =>
            {
                if (account.FramesData.ServerToAutoConnectTo != 0 &&
                    message.Server.Id == account.FramesData.ServerToAutoConnectTo &&
                    (ServerStatusEnum) message.Server.Status == ServerStatusEnum.ONLINE)
                {
                    await Task.Delay(2000);
                    account.Network?.ConnectTimeout?.Change(120000, 120000);
                    account.Logger.LogDebug(LanguageManager.Translate("87"),
                        LanguageManager.Translate("90", message.Server.Name));
                    await account.Network.SendMessageAsync(new ServerSelectionMessage((int) message.Server.Id));
                }
            });
        }

        public static Task HandleSelectedServerDataMessage(Account account, SelectedServerDataMessage message)
        {
            return Task.Run(async () =>
            {
                account.Game.Server.Update(message);

                account.FramesData.Ticket = message.Ticket;
                await account.Network.SwitchToGameServer(message.Address, message.Port, message.ServerId,
                    message.Access);
            });
        }

        public static Task HandleSelectedServerRefusedMessage(Account account, SelectedServerRefusedMessage message)
        {
            return Task.Run(async () =>
            {
                account.Logger.LogError(LanguageManager.Translate("87"),
                    LanguageManager.Translate("632", (ServersListEnum) message.ServerId,
                        (ServerStatusEnum) message.ServerStatus));
                await account.Network.Disconnect("CLIENT_CLOSING");
            });
        }

        public static Task HandleHelloGameMessage(Account account, HelloGameMessage message)
        {
            return Task.Run(async () =>
            {
                account.Logger.LogInfo(LanguageManager.Translate("87"), LanguageManager.Translate("91"));
                await account.Network.SendMessageAsync(
                    new AuthenticationTicketMessage(GlobalConfiguration.Instance.Lang, account.FramesData.Ticket));
                account.Network.Phase = NetworkPhases.GAME;
            });
        }

        public static Task HandleTrustStatusMessage(Account account, TrustStatusMessage message)
        {
            return Task.Run(async () => await account.Network.SendMessageAsync(new CharactersListRequestMessage()));
        }
    }
}