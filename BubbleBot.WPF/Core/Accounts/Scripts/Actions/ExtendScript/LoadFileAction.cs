using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    class LoadFileAction : ScriptAction
    {
        // Properties
        public string FileName { get; private set; }

        // Constructor
        public LoadFileAction(string filename)
        {
            FileName = filename;
        }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            account.Game.ExtendScript.LoadFile(FileName);

            return DoneResult;
        }
    }
}

