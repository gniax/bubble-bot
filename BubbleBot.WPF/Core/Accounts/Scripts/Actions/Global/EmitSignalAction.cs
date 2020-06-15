using BubbleBot.Configurations;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class EmitSignalAction : ScriptAction
    {
        // Constructor
        public EmitSignalAction(string targetId, string msg)
        {
            TargetId = targetId;
            Message = msg;
        }

        // Properties
        public string TargetId { get; }
        public string Message { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            bool success = UInt32.TryParse(TargetId, out uint result);
            if (success)
            {
                foreach (var bot in BubbleBotMain.Instance.EveryConnectedAccount())
                {
                    if (bot.Network.Connected && bot.Game.Character.IsSelected && bot.Game.Character.Id == result)
                    {
                        bot.Scripts.SignalMessage = Message;
                        return ScriptActionResults.DONE;
                    }
                }
            }   
            
            return ScriptActionResults.FAILED;
        }
    }
}