using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Managers.Gathers;
using BubbleBot.Core.Accounts.Scripts.Actions;
using BubbleBot.Core.Accounts.Scripts.Actions.Bid;
using BubbleBot.Core.Accounts.Scripts.Actions.Exchange;
using BubbleBot.Core.Accounts.Scripts.Actions.Fight;
using BubbleBot.Core.Accounts.Scripts.Actions.Gather;
using BubbleBot.Core.Accounts.Scripts.Actions.Global;
using BubbleBot.Core.Accounts.Scripts.Actions.Map;
using BubbleBot.Core.Accounts.Scripts.Actions.Npcs;
using BubbleBot.Core.Enums;
using BubbleBot.Protocol.Messages.Messages;
using BubbleBot.Utility;
using BubbleBot.Utility.Extensions;
using MoonSharp.Interpreter;

namespace BubbleBot.Core.Accounts.Scripts.Managers
{
    public class ActionsManager : IDisposable
    {
        // Fields
        private Account _account;
        private ConcurrentQueue<ScriptAction> _actionsQueue;
        private ScriptAction _currentAction;
        public DynValue _currentCoroutine;
        private int _fightsCounter;
        private int _gathersCounter;
        private bool _mapChanged;
        private TimerWrapper _timeoutTimer;


        // Constructor
        public ActionsManager(Account account)
        {
            _account = account;
            _actionsQueue = new ConcurrentQueue<ScriptAction>();
            _timeoutTimer = new TimerWrapper(60000, Timeout_Callback);

            _account.Game.Map.MapChanged += Map_MapChanged;
            _account.Game.Managers.Movements.MovementFinished += Movements_MovementFinished;
            _account.Game.Managers.Interactives.UseFinished += Interactives_UseFinished;
            _account.Game.Fight.FightJoined += Fight_FightJoined;
            _account.Game.Managers.Gathers.GatherFinished += Gathers_GatherFinished;
            _account.Game.Managers.Gathers.GatherStarted += Gathers_GatherStarted;
            _account.Game.Npcs.QuestionReceived += Npcs_QuestionReceived;
            _account.Game.Npcs.ShopOpened += Npcs_ShopOpened;
            _account.Game.Npcs.ShopItemSelled += Npcs_ShopItemSelled;
            _account.Game.Storage.StorageStarted += Storage_StorageStarted;
            _account.Game.Storage.StorageLeft += Storage_StorageLeft;
            _account.Game.Npcs.DialogLeft += Npcs_DialogLeft;
            _account.Game.Exchange.ExchangeStarted += Exchange_ExchangeStarted;
            _account.Game.Exchange.ExchangeLeft += Exchange_ExchangeLeft;
            _account.Game.Bid.StartedBuying += Bid_StartedBuying;
            _account.Game.Bid.StartedSelling += Bid_StartedSelling;
            _account.Game.Bid.BidLeft += Bid_BidLeft;
            _account.Game.Managers.Teleportables.UseFinished += Teleportables_UseFinished;
        }


        // Properties
        public int FightsOnThisMap { get; private set; }
        public int MonstersGroupToAttack { get; set; }

        // Events
        public event Action<Account, bool> ActionsFinished;
        public event Action<Account, bool> CustomHandled;


        public void HandleCustom(DynValue customFunction)
        {
            if (!_account.Scripts.Running || _currentCoroutine != null)
                return;

            _currentCoroutine = _account.Scripts.ScriptManager.Script.CreateCoroutine(customFunction);
            ProcessCoroutine();
        }

        private void ProcessCoroutine()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentCoroutine.Coroutine.State != CoroutineState.Running)
            {
                try
                {

                    var result = _currentCoroutine.Coroutine.Resume();
                    _account.Logger.LogDebug("Scripts",
                        $"Processing coroutine: (last action: {_currentAction?.GetType().Name}, result: {result}).");

                    // Check if the custom function ended
                    if (result.Type == DataType.Void)
                        //_account.Logger.LogDebug("", "Ending coroutine.");
                        OnCustomHandled();
                }
                catch (Exception ex)
                {
                    //_account.Scripts.StopScript(ex.ToString());
                    _account.Scripts.StopScript(ex.ToString());
                }
            }
        }

        public void EnqueueAction(ScriptAction action, bool startDequeuingActions = false)
        {
            _actionsQueue.Enqueue(action);

            if (startDequeuingActions) DequeueActions(0);

            // If this account is a group chief, enqueue the action to the other members
            // Special case: if there is a coroutube currently being handled, ignore this
            if (_account.HasGroup && _account.IsGroupChief)
                _account.Group.EnqueueActionToMembers(action, startDequeuingActions);
        }

        public void ClearEverything()
        {
            // Clear all the actions in queue
            ClearActions();
            _currentAction = null;
            _currentCoroutine = null;
            _timeoutTimer.Stop();

            // TODO: Ask the users if they want to leave the main counters
            _fightsCounter = 0;
            _gathersCounter = 0;
            FightsOnThisMap = 0;
        }

        public void DequeueActions(int delay, [CallerMemberName] string caller = "")
        {
            Task.Factory.StartNew(async () =>
            {
                //_account.Logger.LogDebug(delay.ToString(), caller + ", waiting..");

                if (_account?.Scripts.Running == false)
                    return;

                if (_timeoutTimer.Enabled)
                    _timeoutTimer.Stop();

                if (delay > 0) await Task.Delay(delay);

                _account.Logger.LogDebug(caller, $"Waited {delay}ms.");

                // If the queue still has actions
                if (_actionsQueue.Count > 0)
                {
                    if (_actionsQueue.TryDequeue(out var action))
                    {
                        _currentAction = action;
                        //_account.Logger.LogDebug("", $"Current action set to: {_currentAction.GetType().Name}.");
                        await ProcessCurrentAction();
                    }
                }
                // Otherwise
                else
                {
                    // If there is a coroutine currently being handled, process it
                    // Otherwise tell the scripts manager that we're done
                    if (_currentCoroutine != null)
                        ProcessCoroutine();
                    else
                        OnActionsFinished();
                }
            }, TaskCreationOptions.LongRunning);
        }

        private async Task ProcessCurrentAction()
        {
            if (!_account.Scripts.Running)
                return;

            var type = _currentAction.GetType().Name;
            _account.Logger.LogDebug("ActionsManager", $"Current action: {type}.");

            switch (await _currentAction.Process(_account))
            {
                case ScriptActionResults.DONE:
                    //_account.Logger.LogDebug("ActionsManager", $"{type} DONE.");
                    DequeueActions(100);
                    break;
                case ScriptActionResults.FAILED:
                    _account.Logger.LogWarning("ActionsManager", $"{type} failed.");
                    break;
                case ScriptActionResults.PROCESSING:
                    _timeoutTimer.Start();
                    break;
            }
        }

        private void Timeout_Callback(object state)
        {
            // Running and not Enabled because during fights, the script is paused
            if (!_account.Scripts.Running)
                return;

            _account.Logger.LogWarning("Scripts", "Timed out.");
            _account.Scripts.StopScript();
            _account.Scripts.StartScript();
        }

        private void ClearActions()
        {
            while (_actionsQueue.TryDequeue(out var temp))
            {
            }

            _currentAction = null;
        }

        private void OnActionsFinished()
        {
            if (_mapChanged)
            {
                _mapChanged = false;
                ActionsFinished?.Invoke(_account, true);
            }
            else
            {
                ActionsFinished?.Invoke(_account, false);
            }
        }

        private void OnCustomHandled()
        {
            _currentCoroutine = null;

            if (_mapChanged)
            {
                _mapChanged = false;
                CustomHandled?.Invoke(_account, true);
            }
            else
            {
                CustomHandled?.Invoke(_account, false);
            }
        }

        #region Received Events

        private void Map_MapChanged()
        {
            if (!_account.Scripts.Running || _currentAction == null)
                return;

            _mapChanged = true;
            //_account.Logger.LogDebug("ActionsManager", $"MapChanged, Action: {_currentAction?.GetType().Name}");

            // Reset FightsOnThisMap if the current action is not a FightAction (since a fight makes you "change the map")
            if (!(_currentAction is FightAction) || !(_currentAction is ForceFightAction))
                FightsOnThisMap = 0;

            // In case the bot gets into a fight (wheiter wanted or not)
            // Added UseAction here because the character can get aggressed in his path and we need to re-run the script after it
            // Added coroutine here also because the character can get into a fight thanks to a custom function (fight() or gather() or even pnj)
            if (!(_currentAction is ChangeMapAction) && !(_currentAction is FightAction) && !(_currentAction is ForceFightAction) &&
                !(_currentAction is GatherAction) && !(_currentAction is UseAction) &&
                _currentCoroutine == null)
                return;

            // WaitMapChangeAction and UseTeleportableAction handle themselves so we ignore this event to not get a double-actions
            if (_currentAction is WaitMapChangeAction || _currentAction is UseTeleportableAction)
                return;

            ClearActions();
            if(_account.Configuration.SpeedHack == true)
            {
                if (!_account.HasGroup)
                    DequeueActions(0);
                else
                    DequeueActions(0);
            }
            else
            {
                if (!_account.HasGroup)
                    DequeueActions(1500);
                else
                    DequeueActions(3000);
            }

        }

        private async void Movements_MovementFinished(bool success)
        {
            if (!_account.Scripts.Running)
                return;

            if ((_currentAction is FightAction || _currentAction is ForceFightAction) && MonstersGroupToAttack != 0)
            {
                if (success)
                {
                    _account.Network.SendMessage(new GameRolePlayAttackMonsterRequestMessage(MonstersGroupToAttack));

                    // Check if the bot got into the fight or not
                    for (var delay = 0; delay < 10000 && _account.State != AccountStates.FIGHTING; delay += 500)
                        await Task.Delay(500);

                    // If not, the group either moved or got stolen from us
                    if (_account.State != AccountStates.FIGHTING)
                    {
                        _account.Logger.LogWarning(LanguageManager.Translate("165"), LanguageManager.Translate("184"));
                        DequeueActions(0);
                    }

                    MonstersGroupToAttack = 0;
                }
                else
                {
                    _account.Scripts.StopScript(LanguageManager.Translate("185"));
                }
            }
            else if (_currentAction is ChangeMapAction && !success)
            {
                _account.Scripts.StopScript(LanguageManager.Translate("477"));
            }
            else if (_currentAction is MoveToCellAction mtca)
            {
                if (success)
                    DequeueActions(0);
                else
                    _account.Scripts.StopScript(LanguageManager.Translate("186", mtca.CellId));
            }
        }

        private void Fight_FightJoined()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is FightAction)
            {
                _timeoutTimer.Stop();

                _fightsCounter++;
                FightsOnThisMap++;

                // Log the counter only if the script says so
                if (_account.Scripts.ScriptManager.GetGlobalOr(LanguageManager.Translate("187"), DataType.Boolean,
                    false))
                    _account.Logger.LogInfo(LanguageManager.Translate("165"),
                        LanguageManager.Translate("188", _fightsCounter));
            }
        }

        private void Gathers_GatherFinished(GatherResults result)
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is GatherAction)
                switch (result)
                {
                    case GatherResults.FAILED:
                        _account.Scripts.StopScript(LanguageManager.Translate("189"));
                        break;
                    default: // GATHERED, STOLEN, BLACKLISTED or TIMED_OUT
                        DequeueActions(1000);
                        break;
                }
        }

        private void Gathers_GatherStarted()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is GatherAction)
            {
                _gathersCounter++;

                // Log the counter only if the script says so
                if (_account.Scripts.ScriptManager.GetGlobalOr(LanguageManager.Translate("190"), DataType.Boolean,
                    false))
                    _account.Logger.LogInfo(LanguageManager.Translate("165"),
                        LanguageManager.Translate("191", _gathersCounter));
            }
        }

        private void Interactives_UseFinished(bool success)
        {
            if (!_account.Scripts.Running)
                return;

            // If the current action is a UseDoor, then ignore this and wait for a changeMap
            if (_currentAction is UseAction || _currentAction is UseByIdAction || _currentAction is SaveZaapAction ||
                _currentAction is UseLockedHouseAction)
            {
                if (!success)
                    _account.Scripts.StopScript(LanguageManager.Translate("192"));
                else
                    // If there are still actions in the queue (99% it's a WaitMapChange)
                    DequeueActions(_actionsQueue.Count > 0 ? 0 : 500);
            }
        }

        private void Npcs_QuestionReceived()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is NpcBankAction nba)
            {
                if (!_account.Game.Npcs.Reply(nba.ReplyId))
                    _account.Scripts.StopScript(LanguageManager.Translate("193", nba.ReplyId));
            }
            else if (_currentAction is NpcAction || _currentAction is ReplyAction)
            {
                DequeueActions(400);
            }
        }
        private void Npcs_ShopOpened()
        {
            if (!_account.Scripts.Running)
                return;

           if (_currentAction is NpcOpenShopAction) DequeueActions(400);
        }

        private void Npcs_ShopItemSelled()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is NpcShopSellItemAction) DequeueActions(400);
        }
        
        private void Npcs_DialogLeft()
        {
            if (!_account.Scripts.Running)
                return;

            // Also dequeue in case it's an ReplyAction because sometimes the dialog is left without requesting it
            if (_currentAction is ReplyAction || _currentAction is LeaveDialogAction) DequeueActions(200);
        }

        private void Storage_StorageStarted()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is NpcBankAction || _currentAction is UseLockedStorageAction)
            {
                _account.Logger.LogInfo(LanguageManager.Translate("165"), LanguageManager.Translate("194"));
                DequeueActions(400);
            }
        }

        private void Storage_StorageLeft()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is LeaveDialogAction)
            {
                _account.Logger.LogInfo(LanguageManager.Translate("165"), LanguageManager.Translate("195"));
                DequeueActions(400);
            }
        }

        private void Exchange_ExchangeStarted()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is StartExchangeAction)
            {
                _account.Logger.LogInfo(LanguageManager.Translate("165"), LanguageManager.Translate("196"));
                DequeueActions(400);
            }
        }

        private void Exchange_ExchangeLeft()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is LeaveDialogAction || _currentAction is SendReadyAction)
            {
                _account.Logger.LogInfo(LanguageManager.Translate("165"), LanguageManager.Translate("197"));
                DequeueActions(400);
            }
        }

        private void Bid_StartedSelling()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is StartSellingAction) DequeueActions(400);
        }

        private void Bid_StartedBuying()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is StartBuyingAction) DequeueActions(400);
        }

        private void Bid_BidLeft()
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is LeaveDialogAction) DequeueActions(400);
        }

        private void Teleportables_UseFinished(bool success)
        {
            if (!_account.Scripts.Running)
                return;

            if (_currentAction is UseTeleportableAction uta)
            {
                if (success)
                    DequeueActions(1500);
                else
                    _account.Scripts.StopScript(LanguageManager.Translate("540", uta.Type.ToString().PureCapitalize()));
            }
        }

        #endregion

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) _timeoutTimer.Dispose();

                _actionsQueue = null;
                _currentAction = null;
                _account = null;
                _timeoutTimer = null;

                _disposedValue = true;
            }
        }

        ~ActionsManager()
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