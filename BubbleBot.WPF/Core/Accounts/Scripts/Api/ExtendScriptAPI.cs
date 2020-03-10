using BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript;
using MoonSharp.Interpreter;
using System;
using System.Reflection;

namespace BubbleBot.Core.Accounts.Scripts.Api
{
    [MoonSharpUserData]
    [Obfuscation(Exclude = false, Feature = "-rename", ApplyToMembers = true)]
    public class ExtendScriptAPI : IDisposable
    {
        // Fields
        private Account _account;

        // Constructor
        public ExtendScriptAPI(Account account)
        {
            _account = account;
        }


        public void CreateFile(string filename)
        {
            _account.Game.ExtendScript.ExtScriptMode = true;
            _account.Scripts.ActionsManager.EnqueueAction(new CreateFileAction((string)filename), true);
            
        }

        public void DeleteFile(string filename)
            => _account.Scripts.ActionsManager.EnqueueAction(new DeleteFileAction((string)filename), true);

        public void LoadFile(string filename)
             => _account.Scripts.ActionsManager.EnqueueAction(new LoadFileAction((string)filename), true);

        public void EditValue(string filename,string name, int value)
            => _account.Scripts.ActionsManager.EnqueueAction(new EditValueAction(filename, name, value), true);

        public int GetValue(string name)
            => _account.Game.ExtendScript.GetValue(name);


        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _account = null;


                disposedValue = true;
            }
        }

        ~ExtendScriptAPI()
            => Dispose(false);

        public void Dispose()
            => Dispose(true);

        #endregion
    }
}
