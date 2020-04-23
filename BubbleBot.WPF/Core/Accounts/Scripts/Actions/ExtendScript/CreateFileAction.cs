using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    internal class CreateFileAction : ScriptAction
    {
        // Constructor
        public CreateFileAction(string filename)
        {
            FileName = filename;
        }

        // Properties
        public string FileName { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.ExtendScript.CreateFile(FileName)) await Task.Delay(700);

            return ScriptActionResults.DONE;
        }
    }
}