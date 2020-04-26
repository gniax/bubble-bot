using System;
using System.Collections.Generic;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Messages;
using BubbleBot.Protocol.Types;
using BubbleBot.Data;

namespace BubbleBot.Core.Accounts.InGame.Server
{
    public class ServerGame : IDisposable
    {
        // Constructor
        internal ServerGame()
        {
            Characters = new List<CharacterBaseInformations>();
        }

        // Properties
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<CharacterBaseInformations> Characters { get; private set; }


        // Events
        public event Action ServerSelected;


        #region Update

        public void Update(SelectedServerDataMessage message)
        {
            Id = message.ServerId;
            Name = DataManager.Get<Servers>(Id).NameId;
            Console.WriteLine(Name);

            ServerSelected?.Invoke();
        }

        public void Update(CharactersListMessage message)
        {
            if (message.Characters.Count > 0) Characters.AddRange(message.Characters);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                Name = null;
                Characters.Clear();
                Characters = null;

                disposedValue = true;
            }
        }

        ~ServerGame()
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