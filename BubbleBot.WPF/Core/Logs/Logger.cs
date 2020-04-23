using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using BubbleBot.Configurations;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Core.Logs
{
    public class Logger : IDisposable
    {
        // Properties
        private readonly Account _account;

        // Constructor
        public Logger(Account account)
        {
            _account = account;
            Logs = new ObservableCollection<LogMessage>();
        }

        public ObservableCollection<LogMessage> Logs { get; private set; }


        public void Log(string source, string message, string color, List<ObjectItem> objects = null)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (Logs.Count >= 200)
                    Logs.RemoveAt(0);

                Logs.Add(new LogMessage(source, message, $"#ff{color}", objects == null ? null : objects));
            });
        }


        public void LogDebug(string source, string message)
        {
            Log(source, message, LogTypes.DEBUG);
        }

        public void LogError(string source, string message)
        {
            Log(source, message, LogTypes.ERROR);
        }

        public void LogInfo(string source, string message)
        {
            Log(source, message, LogTypes.INFO);
        }

        public void LogWarning(string source, string message)
        {
            Log(source, message, LogTypes.WARNING);
        }

        public void LogFight(string source, string message)
        {
            Log(source, message, LogTypes.FIGHT);
        }

        public void LogDofus(string source, string message)
        {
            Log(source, message, LogTypes.DOFUS);
        }

        public void LogMessage(string source, string message)
        {
            Log(source, message, LogTypes.MESSAGE);
        }

        private void Log(string source, string message, LogTypes type)
        {
            if (type == LogTypes.DEBUG && !GlobalConfiguration.Instance.ShowDebugMessages)
                return;

            if (type == LogTypes.FIGHT && !_account.Configuration.ShowFightMessages)
                return;

            Log(source, message, ((int) type).ToString("X6"));
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                Application.Current?.Dispatcher.Invoke(() => Logs.Clear());
                Logs = null;

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}