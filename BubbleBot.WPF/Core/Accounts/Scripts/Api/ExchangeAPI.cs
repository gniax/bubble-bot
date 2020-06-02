using System;
using System.Reflection;
using BubbleBot.Core.Accounts.Scripts.Actions.Exchange;
using MoonSharp.Interpreter;

namespace BubbleBot.Core.Accounts.Scripts.Api
{
    [MoonSharpUserData]
    [Obfuscation(Exclude = false, Feature = "-rename", ApplyToMembers = true)]
    public class ExchangeAPI : IDisposable
    {
        // Fields
        private Account _account;


        // Constructor
        public ExchangeAPI(Account account)
        {
            _account = account;
        }


        public int WeightP()
        {
            return _account.Game.Exchange.WeightPercent;
        }

        public int TargetWeightP()
        {
            return _account.Game.Exchange.RemoteWeightPercent;
        }

        public void StartExchange(uint playerId)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new StartExchangeAction((int) playerId), true);
        }

        public void AddAccountManagerId()
        {
            _account.Scripts.ActionsManager.EnqueueAction(new ExchangeAddAccountManager(), true);
        }

        public void StartExchangeByName(string playerName)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new StartExchangeByNameAction(playerName), true);
        }

        public void StartExchangeGroupByName(string playerName)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new StartExchangeGroupByNameAction(playerName), true);
            //return _account.Game.Exchange.StartExchangeGroupByName(playerName);
            //_account.Scripts.ActionsManager.EnqueueAction(new StartExchangeGroupByNameAction(playerName), true);
        }

        public void SendReady()
        {
            _account.Scripts.ActionsManager.EnqueueAction(new SendReadyAction(), true);
        }

        public void PutItem(int gid, uint qty)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new ExchangePutItemAction(gid, qty), true);
        }

        public void RemoveItem(int gid, uint qty)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new ExchangeRemoveItemAction(gid, qty), true);
        }

        public void PutAllItems()
        {
            _account.Scripts.ActionsManager.EnqueueAction(new ExchangePutAllItemsAction(), true);
        }

        public void PutKamas(uint qty)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new ExchangePutKamasAction(qty), true);
        }

        public void RemoveKamas(uint qty)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new ExchangeRemoveKamasAction(qty), true);
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _account = null;


                disposedValue = true;
            }
        }

        ~ExchangeAPI()
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