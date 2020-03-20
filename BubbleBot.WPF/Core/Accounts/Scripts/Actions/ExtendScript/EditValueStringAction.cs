using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    class EditValueStringAction : ScriptAction
    {
        // Properties
        public string FileName { get; private set; }
        public string Name { get; private set; }
        public string Value { get; private set; }

        // Constructor
        public EditValueStringAction(string filename, string name, string value)
        {
            FileName = filename;
            Name = name;
            Value = value;
        }


        internal override async Task<ScriptActionResults> Process(Account account)
        {

            if (account.Game.ExtendScript.EditValueString(FileName, Name, Value))
            {
                await Task.Delay(1);
            }

            return ScriptActionResults.DONE;
        }
    }
}
