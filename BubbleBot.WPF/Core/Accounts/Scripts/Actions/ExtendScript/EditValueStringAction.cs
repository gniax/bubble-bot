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


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return DoneResult;

            if (account.Game.ExtendScript.EditValueString(FileName, Name, Value).Result) 
                Task.Delay(200);

            return DoneResult;
        }
    }
}