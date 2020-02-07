using BubbleBot.Protocol.Messages;
using System;
using System.Threading.Tasks;
using BubbleBot.Core.Accounts.Extensions;
using BubbleBot.Core.Accounts.Scripts.Managers;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    class MapTeleportationAction : ScriptAction
    {
        internal async override Task<ScriptActionResults> Process(Account account)
        {
                return ScriptActionResults.PROCESSING;
            
        }
    }
}
