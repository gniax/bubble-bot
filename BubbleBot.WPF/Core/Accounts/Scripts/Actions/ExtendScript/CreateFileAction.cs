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


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return DoneResult;

            if (account.Game.ExtendScript.CreateFile(FileName).Result) 
                Task.Delay(700);

            return DoneResult;
        }
    }
}