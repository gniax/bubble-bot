using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    class EditValueIntAction : ScriptAction
    {
        // Properties
        public string FileName { get; private set; }
        public string Name { get; private set; }
        public int Value { get; private set; }

        // Constructor
        public EditValueIntAction(string filename,string name,int value)
        {
            FileName = filename;
            Name = name;
            Value = value;
        }


        internal override async Task<ScriptActionResults> Process(Account account)
        {

            if (account.Game.ExtendScript.EditValueInt(FileName, Name, Value))
            {
                await Task.Delay(1);
            }

            return ScriptActionResults.DONE;
        }
    }
}
