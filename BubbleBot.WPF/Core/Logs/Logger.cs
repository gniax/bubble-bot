using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using BubbleBot.Configurations;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Enums;
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
            if (GlobalConfiguration.Instance.FormattingLogs)
            {
                message = FormatContent(message);
            }

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

            if (GlobalConfiguration.Instance.FormattingLogs)
            {
                message = FormatContent(message);
            }

            Log(source, message, ((int) type).ToString("X6"));
        }

        private static string FormatContent(string message)
        {
            if (message.Contains("<br />"))
                message = message.Replace("<br />", "");

            if (message.Contains("&lt;"))
                message = message.Replace("&lt;", "<");

            if (message.Contains("&gt;"))
                message = message.Replace("&gt;", ">");

            if (message.Contains("&amp;"))
                message = message.Replace("&amp;", "&");

            if (message.Contains("&quot;"))
                message = message.Replace("&quot;", "\"");

            if (message.Contains("{openSocial,0,0::ami(s)}"))
            {
                message = message.Replace("{openSocial,0,0::ami(s)}", "ami(s)");
            }

            string playerPattern = @"\(\{player,(\D+)\,(\d+)\}\)";
            var regMatch = Regex.Match(message, playerPattern);
            if (regMatch.Success)
            {
                message = message.Replace(regMatch.Value, regMatch.Groups[1].Value);
            }

            string itemPattern0 = @"\{item\,(\d{1,5})\,\d+}";
            regMatch = Regex.Match(message, itemPattern0);
            if (regMatch.Success)
            {
                var itemname = ObjectEnumFinder.GetObjectNameById(Int32.Parse(regMatch.Groups[1].Value));
                if (itemname != null)
                    message = message.Replace(regMatch.Value, itemname);
            }

            string challengePattern = @"\$challenge(\d{1,2})";
            regMatch = Regex.Match(message, challengePattern);
            if (regMatch.Success)
            {
                message = message.Replace(regMatch.Value, ChallengesEnumFinder.GetChallengeById(Int32.Parse(regMatch.Groups[1].Value)));
            }

            string itemPattern = @"\$item(\d{1,5})";
            regMatch = Regex.Match(message, itemPattern);
            if (regMatch.Success)
            {
                var itemname = ObjectEnumFinder.GetObjectNameById(Int32.Parse(regMatch.Groups[1].Value));
                if (itemname != null)
                    message = message.Replace(regMatch.Value, itemname);
            }

            string mapPattern = @"\{mapWithFlag,(.*),\d\}";
            regMatch = Regex.Match(message, mapPattern);
            if (regMatch.Success)
            {
                message = message.Replace(regMatch.Value, $"[{regMatch.Groups[1].Value}]");
            }

            return message;
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