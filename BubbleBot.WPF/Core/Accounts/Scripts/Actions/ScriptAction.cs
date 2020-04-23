using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions
{
    public abstract class ScriptAction
    {
        protected static Task<ScriptActionResults> DoneResult => Task.FromResult(ScriptActionResults.DONE);
        protected static Task<ScriptActionResults> ProcessingResult => Task.FromResult(ScriptActionResults.PROCESSING);
        protected static Task<ScriptActionResults> FailedResult => Task.FromResult(ScriptActionResults.FAILED);


        internal abstract Task<ScriptActionResults> Process(Account account);
    }

    public enum ScriptActionResults
    {
        DONE,
        PROCESSING,
        FAILED
    }
}