using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    internal class MapTeleportationAction : ScriptAction
    {
        internal override async Task<ScriptActionResults> Process(Account account)
        {
            await Task.Delay(1);
            return ScriptActionResults.PROCESSING;
        }
    }
}