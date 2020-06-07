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


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return DoneResult;

            if (account.Game.ExtendScript.DeleteFile(FileName).Result) 
                Task.Delay(700);

            return DoneResult;
        }
    }
}