using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript
{
    class GetValueAction : ScriptAction
    {
        // Properties
        public string FileName { get; private set; }
        public string Name { get; private set; }

        // Constructor
        public GetValueAction(string name)
        {
            Name = name;
        }

        internal override Task<ScriptActionResults> Process(Account account)
        {

            account.Game.ExtendScript.GetValue(Name);
            Thread.Sleep(1000);
            return DoneResult;
        }
    }
}
