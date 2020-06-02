using System;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Accounts.Extensions.Exchanges
{
    public class RoleplayExtension : IDisposable
    {
        // Fields
        private Account _account;


        // Constructor
        public RoleplayExtension(Account account)
        {
            _account = account;

            _account.Game.Exchange.ExchangeRequested += Exchange_ExchangeRequested;
            _account.Game.Exchange.RemoteReady += Exchange_RemoteReady;
            _account.Network.RegisterMessage<GameRolePlayPlayerFightFriendlyRequestedMessage>(
                HandleGameRolePlayPlayerFightFriendlyRequestedMessage);
        }


        private void Exchange_ExchangeRequested(int from)
        {
            var defautAuthorized = false;

            //Si un personnage du bot ajoute son id il est accepté pour l'échange
            foreach (var playerIdTmp in _account.Game.Exchange.AuthorizedPlayersList)
                if (playerIdTmp == from)
                    defautAuthorized = true;

            if (_account.Configuration.AcceptBotsTrades && BubbleBotMain.Instance.ConnectedAccounts.Select(c => c.Game?.Character.Id == from).Any())
            {
                _account.Network.SendMessage(new ExchangeAcceptMessage());
                _account.Logger.LogInfo(LanguageManager.Translate("117"), LanguageManager.Translate("384"));
                return;
            }
            // If this character isn't authorized to trade us, refuse it
            if (!_account.Configuration.AuthorizedTradesFrom.Contains(from) && defautAuthorized == false)
            {
                if (_account.Configuration.IgnoreNonAuthorizedTrades)
                {
                    var player = _account.Game.Map.GetPlayer(from);

                    if (player != null)
                    {
                        _account.Network.SendMessage(new IgnoredAddRequestMessage(player.Name, true));
                        _account.Network.SendMessage(new LeaveDialogRequestMessage());
                        _account.Logger.LogWarning(LanguageManager.Translate("117"), LanguageManager.Translate("382"));
                        return;
                    }
                }

                // If the IgnoreNonAuthorizedTrades option is disabled or the player wasn't found on the map (somehow)
                _account.Network.SendMessage(new LeaveDialogRequestMessage());
                _account.Logger.LogWarning(LanguageManager.Translate("117"), LanguageManager.Translate("383"));
            }
            // Otherwise accept it
            else
            {
                _account.Network.SendMessage(new ExchangeAcceptMessage());
                _account.Logger.LogInfo(LanguageManager.Translate("117"), LanguageManager.Translate("384"));
            }
        }

        private void Exchange_RemoteReady()
        {
            _account.Game.Exchange.SendReady();
        }

        private Task HandleGameRolePlayPlayerFightFriendlyRequestedMessage(Account account,
            GameRolePlayPlayerFightFriendlyRequestedMessage message)
        {
            return Task.Run(async () =>
            {
                if (message.TargetId != account.Game.Character.Id)
                    return;

                await Task.Delay(1000);

                var player = account.Game.Map.GetPlayer((int) message.SourceId);

                if (player != null)
                {
                    await account.Network.SendMessageAsync(
                        new GameRolePlayPlayerFightFriendlyAnswerMessage((int) message.FightId));
                    _account.Network.SendMessage(new IgnoredAddRequestMessage(player.Name, true));
                    _account.Network.SendMessage(new LeaveDialogRequestMessage());
                    account.Logger.LogWarning(LanguageManager.Translate("553"), LanguageManager.Translate("554"));
                }
            });
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _account = null;

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}