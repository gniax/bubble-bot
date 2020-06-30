using System;
using System.Reflection;
using BubbleBot.Core.Accounts.Scripts.Actions.ExtendScript;
using MoonSharp.Interpreter;

namespace BubbleBot.Core.Accounts.Scripts.Api
{
    [MoonSharpUserData]
    [Obfuscation(Exclude = false, Feature = "-rename", ApplyToMembers = true)]
    public class ExtendScriptAPI : IDisposable
    {
        // Fields
        public Account _account;

        // Constructor
        public ExtendScriptAPI(Account account)
        {
            _account = account;
        }


        public void CreateFile(string filename)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new CreateFileAction(filename));
        }

        public void DeleteFile(string filename)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new DeleteFileAction(filename));
        }

        public void EditValueInt(string filename, string name, int value)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new EditValueIntAction(filename, name, value));
        }

        public void EditValueString(string filename, string name, string value)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new EditValueStringAction(filename, name, value));
        }

        public void DeleteVariable(string filename, string name)
        {
            _account.Scripts.ActionsManager.EnqueueAction(new DeleteVariable(filename, name));
        }

        public int GetValueInt(string filename, string name)
        {
            return _account.Game.ExtendScript.GetValueInt(filename, name).Result;
        }

        public string GetValueString(string filename, string name)
        {
            return _account.Game.ExtendScript.GetValueString(filename, name).Result;
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _account = null;


                disposedValue = true;
            }
        }

        ~ExtendScriptAPI()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}