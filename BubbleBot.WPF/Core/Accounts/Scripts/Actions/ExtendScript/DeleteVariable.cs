using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    internal class DeleteVariable : ScriptAction
    {
        // Constructor
        public DeleteVariable(string filename, string name)
        {
            FileName = filename;
            Name = name;
        }

        // Properties
        public string FileName { get; }
        public string Name { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.ExtendScript.DeleteVariable(FileName, Name).Result) 
                Task.Delay(100);

            return DoneResult;
        }
    }
}