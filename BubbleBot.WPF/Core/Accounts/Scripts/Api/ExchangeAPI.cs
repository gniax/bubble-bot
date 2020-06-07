using System;
using System.Linq;
using System.Reflection;
using BubbleBot.Core.Accounts.Scripts.Actions.Exchange;
using BubbleBot.Core.Enums;
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

        public int TargetPlayerId()
        {
            return _account.Game.Exchange.RemoteCharacterId;
        }
        public bool IsInExchange()
        {
            if (_account.State == AccountStates.EXCHANGE)
                return true;
            else
                return false;
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

        public void FromBotExchangeByName(string groupMng, string idMng, string playerName)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new FromBotExchangeByNameAction(groupMng, idMng, playerName), true);
        }
        public void FromBotPutAllItems(string groupMng, string idMng)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new FromBotExchangePutAllItemsAction(groupMng, idMng), true);
        }
        public void FromBotPutItem(string groupMng, string idMng, int gid, uint qty)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new FromBotExchangePutItemAction(groupMng, idMng, gid, qty), true);
        }
        public void FromBotPutKamas(string groupMng, string idMng, uint qty)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new FromBotExchangePutKamasAction(groupMng, idMng, qty), true);
        }

        public void FromBotSendReady(string groupMng, string idMng)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new FromBotSendReadyAction(groupMng, idMng), true);
        }
        public bool FromBotIsInExchange(string groupMng, string idMng)
        {
            foreach (var acc in BubbleBotMain.Instance.ConnectedAccounts)
            {
                //Si le compte est en groupe on vérifie les membres
                if (acc.IsGroupChief && acc.HasGroup)
                {
                    foreach (var member in acc.Group.Members)
                    {
                        if (member.AccountConfig.Nickname == groupMng && member.AccountConfig.Identifiant == idMng)
                        {
                            if (member.State == AccountStates.EXCHANGE)
                                return true;
                            else
                                return false;
                        }
                    }
                }
                //On vérifie les comptes principal
                if (acc.AccountConfig.Nickname == groupMng && acc.AccountConfig.Identifiant == idMng)
                {
                    if (acc.State == AccountStates.EXCHANGE)
                        return true;
                    else
                        return false;
                }
            }
            return false;
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