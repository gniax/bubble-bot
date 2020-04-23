using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class WaitMapChangeAction : ScriptAction
    {
        // Constructor
        public WaitMapChangeAction(uint delay)
        {
            Delay = delay;
        }

        // Properties
        public uint Delay { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            account.Logger.LogDebug("WaitMapChangeAction", "Waiting");
            var mapChanged = false;

            void Map_MapChanged()
            {
                mapChanged = true;
            }

            account.Game.Map.MapChanged += Map_MapChanged;

            var delay = 0;
            while (!mapChanged && delay < Delay && account.Scripts.Running)
            {
                await Task.Delay(100);
                delay += 100;
            }

            account.Game.Map.MapChanged -= Map_MapChanged;

            if (!mapChanged && delay == Delay)
                account.Logger.LogWarning("", "WaitMapChange timed out.");

            account.Logger.LogDebug("WaitMapChangeAction", "Waited");
            return ScriptActionResults.DONE;
        }
    }
}