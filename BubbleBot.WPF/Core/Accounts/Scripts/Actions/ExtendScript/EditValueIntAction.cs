using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    internal class EditValueIntAction : ScriptAction
    {
        // Constructor
        public EditValueIntAction(string filename, string name, int value)
        {
            FileName = filename;
            Name = name;
            Value = value;
        }

        // Properties
        public string FileName { get; }
        public string Name { get; }
        public int Value { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return DoneResult;

            if (account.Game.ExtendScript.EditValueInt(FileName, Name, Value).Result) 
                Task.Delay(200);

            return DoneResult;
        }
    }
}