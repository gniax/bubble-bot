using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    class EditValueAction : ScriptAction
    {
        // Properties
        public string FileName { get; private set; }
        public string Name { get; private set; }
        public int Value { get; private set; }

        // Constructor
        public EditValueAction(string filename,string name,int value)
        {
            FileName = filename;
            Name = name;
            Value = value;
        }


        internal override Task<ScriptActionResults> Process(Account account)
        {

            account.Game.ExtendScript.EditValue(FileName, Name, Value);

            return DoneResult;
        }
    }
}
