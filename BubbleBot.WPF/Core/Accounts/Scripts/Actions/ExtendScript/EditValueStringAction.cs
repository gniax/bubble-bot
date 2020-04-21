using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    internal class EditValueStringAction : ScriptAction
    {
        // Constructor
        public EditValueStringAction(string filename, string name, string value)
        {
            FileName = filename;
            Name = name;
            Value = value;
        }

        // Properties
        public string FileName { get; }
        public string Name { get; }
        public string Value { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.ExtendScript.EditValueString(FileName, Name, Value)) await Task.Delay(1);

            return ScriptActionResults.DONE;
        }
    }
}