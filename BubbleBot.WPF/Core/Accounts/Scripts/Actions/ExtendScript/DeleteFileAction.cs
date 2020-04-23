using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    internal class DeleteFileAction : ScriptAction
    {
        // Constructor
        public DeleteFileAction(string filename)
        {
            FileName = filename;
        }

        // Properties
        public string FileName { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.ExtendScript.DeleteFile(FileName)) await Task.Delay(700);

            return ScriptActionResults.DONE;
        }
    }
}