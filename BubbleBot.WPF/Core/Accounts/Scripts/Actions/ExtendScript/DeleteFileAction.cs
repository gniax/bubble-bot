using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    class DeleteFileAction : ScriptAction
    {
        // Properties
        public string FileName { get; private set; }


        // Constructor
        public DeleteFileAction(string filename)
        {
            FileName = filename;
        }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.ExtendScript.DeleteFile(FileName))
            {
                await Task.Delay(500);
            }

            return ScriptActionResults.DONE;
        }
    }
}
