using BubbleBot.Protocol.Messages;
using System;
using System.Threading.Tasks;
using BubbleBot.Core.Accounts.Extensions;
using BubbleBot.Core.Accounts.Scripts.Managers;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    class GuidedModeQuit : ScriptAction
    {
        internal async override Task<ScriptActionResults> Process(Account account)
        {
            BubbleBot.Core.Accounts.Extensions.CharacterCreator.CharacterCreatorExtension.ActionStartTutorial(account);
            bool tutorial = System.Threading.SpinWait.SpinUntil(() => (account.Extensions.CharacterCreation._terminated), TimeSpan.FromSeconds(60));

            if (tutorial)
            {
                bool mapchanged = System.Threading.SpinWait.SpinUntil(() => (account.Extensions.CharacterCreation._mapchanged), TimeSpan.FromSeconds(10));
                if (mapchanged)
                {
                    await Task.Delay(1000);
                    account.Scripts.StartScript();
                    return ScriptActionResults.PROCESSING;
                }
                else
                    return ScriptActionResults.FAILED;
            }
            else
            {
                return ScriptActionResults.FAILED;           
            }
        }
    }
}
