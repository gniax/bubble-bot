using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class DelayAction : ScriptAction
    {
        // Constructor
        public DelayAction(int ms)
        {
            Milliseconds = ms;
        }

        // Properties
        public int Milliseconds { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            await Task.Delay(Milliseconds);
            return ScriptActionResults.DONE;
        }
    }
}