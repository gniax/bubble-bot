using System;
using System.Threading.Tasks;
using BubbleBot.Protocol.Enums;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Accounts.InGame.Chat
{
    public class ChatGame : IDisposable
    {
        // Fields
        private Account _account;


        // Constructor
        public ChatGame(Account account)
        {
            _account = account;
        }


        public async Task SendMessage(string message,
            ChatActivableChannelsEnum channel = ChatActivableChannelsEnum.CHANNEL_GLOBAL)
        {
            if (string.IsNullOrEmpty(message))
                return;

            await _account.Network.SendMessageAsync(new ChatClientMultiMessage(message, (uint) channel));
        }

        public async Task SendMessageTo(string message, string receiver)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(receiver))
                return;

            await _account.Network.SendMessageAsync(new ChatClientPrivateMessage(message, receiver));
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                _account = null;

                disposedValue = true;
            }
        }

        ~ChatGame()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}