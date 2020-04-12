using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    class DeleteVariable : ScriptAction
    {
        // Properties
        public string FileName { get; private set; }
        public string Name { get; private set; }

        // Constructor
        public DeleteVariable(string filename, string name)
        {
            FileName = filename;
            Name = name;
        }

        internal override async Task<ScriptActionResults> Process(Account account)
        {

            if (account.Game.ExtendScript.DeleteVariable(FileName, Name))
            {
                await Task.Delay(1);
            }

            return ScriptActionResults.DONE;
        }
    }
}
