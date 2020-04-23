using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using BubbleBot.Configurations.Language;
using GalaSoft.MvvmLight;

namespace BubbleBot.Core.Accounts.Extensions.Bid
{
    public class BidConfiguration : ViewModelBase, IDisposable
    {
        // Fields
        private const string configurationsPath = @"Parameters\Bid";
        private Account _account;
        private int _interval;
        private bool _loaded;
        private string _scriptPath;


        // Constructor
        public BidConfiguration(Account account)
        {
            _account = account;
            ObjectsToSell = new ObservableCollection<ObjectToSellEntry>();
        }


        // Properties
        public int Interval
        {
            get => _interval;
            set
            {
                Set(ref _interval, value);
                Save();
            }
        }

        public string ScriptPath
        {
            get => _scriptPath;
            set
            {
                Set(ref _scriptPath, value);
                Save();
            }
        }

        public ObservableCollection<ObjectToSellEntry> ObjectsToSell { get; }

        private string ConfigFilePath => Path.Combine(configurationsPath,
            LanguageManager.Translate("68", _account.AccountConfig.Username, _account.Game.Character.Name));

        public bool IsScriptPathValid => !string.IsNullOrEmpty(ScriptPath);


        public void Load()
        {
            _loaded = false;

            if (File.Exists(ConfigFilePath))
                using (var br = new BinaryReader(File.Open(ConfigFilePath, FileMode.Open, FileAccess.ReadWrite,
                    FileShare.ReadWrite)))
                {
                    Interval = br.ReadInt32();
                    ScriptPath = br.ReadString();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ObjectsToSell.Clear();
                        var c = br.ReadByte();
                        for (var i = 0; i < c; i++)
                            ObjectsToSell.Add(ObjectToSellEntry.Load(br));
                    });
                }
            else
                Interval = 10;

            _loaded = true;
        }

        public void Save()
        {
            // Avoid saving when we're loading
            if (!_loaded)
                return;

            // Ensure that the configuration directory is there
            Directory.CreateDirectory(configurationsPath);

            using (var bw = new BinaryWriter(File.Open(ConfigFilePath, FileMode.Create, FileAccess.ReadWrite,
                FileShare.ReadWrite)))
            {
                bw.Write(Interval);
                bw.Write(ScriptPath ?? "");

                bw.Write((byte) ObjectsToSell.Count);
                foreach (var obj in ObjectsToSell)
                    obj.Save(bw);
            }
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _scriptPath = null;
                _account = null;

                disposedValue = true;
            }
        }

        ~BidConfiguration()
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