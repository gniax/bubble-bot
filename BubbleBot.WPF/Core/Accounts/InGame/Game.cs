using System;
using BubbleBot.Core.Accounts.InGame.Bid;
using BubbleBot.Core.Accounts.InGame.Character;
using BubbleBot.Core.Accounts.InGame.Chat;
using BubbleBot.Core.Accounts.InGame.Exchange;
using BubbleBot.Core.Accounts.InGame.ExtendScript;
using BubbleBot.Core.Accounts.InGame.Fights;
using BubbleBot.Core.Accounts.InGame.Managers;
using BubbleBot.Core.Accounts.InGame.Map;
using BubbleBot.Core.Accounts.InGame.Npcs;
using BubbleBot.Core.Accounts.InGame.Server;
using BubbleBot.Core.Accounts.InGame.Storage;

namespace BubbleBot.Core.Accounts.InGame
{
    public class Game : IClearable, IDisposable
    {
        public double bakRate = 0;
        public int shopBuyInfo = 0;

        // Constructor
        internal Game(Account account)
        {
            Server = new ServerGame();
            Character = new CharacterGame(account);
            Map = new MapGame(account);
            Fight = new FightGame(account);
            Managers = new ManagersGame(account, Map);
            Chat = new ChatGame(account);
            Npcs = new NpcsGame(account);
            Storage = new StorageGame(account);
            Exchange = new ExchangeGame(account);
            Bid = new BidGame(account);
            ExtendScript = new ExtendScriptGame(account);
        }

        // Properties
        public ServerGame Server { get; private set; }
        public CharacterGame Character { get; private set; }
        public MapGame Map { get; private set; }
        public ManagersGame Managers { get; private set; }
        public FightGame Fight { get; private set; }
        public ChatGame Chat { get; private set; }
        public NpcsGame Npcs { get; private set; }
        public StorageGame Storage { get; private set; }
        public ExchangeGame Exchange { get; private set; }
        public BidGame Bid { get; private set; }
        public ExtendScriptGame ExtendScript { get; private set; }


        public void Clear()
        {
            Character.Clear();
            Map.Clear();
            Fight.Clear();
            Managers.Clear();
            Bid.Clear();
            ExtendScript.Clear();
        }


        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Server.Dispose();
                    Character.Dispose();
                    Map.Dispose();
                    Fight.Dispose();
                    Managers.Dispose();
                    Chat.Dispose();
                    Npcs.Dispose();
                    Storage.Dispose();
                    Exchange.Dispose();
                    Bid.Dispose();
                    ExtendScript.Dispose();
                }

                Server = null;
                Character = null;
                Map = null;
                Fight = null;
                Managers = null;
                Chat = null;
                Npcs = null;
                Storage = null;
                Exchange = null;
                Bid = null;
                ExtendScript = null;

                disposedValue = true;
            }
        }

        ~Game()
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