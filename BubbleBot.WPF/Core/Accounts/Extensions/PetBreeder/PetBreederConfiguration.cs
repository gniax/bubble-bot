using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using BubbleBot.Configurations.Language;
using GalaSoft.MvvmLight;

namespace BubbleBot.Core.Accounts.Extensions.PetBreeder
{
    public class PetBreederConfiguration : IDisposable
    {
        // Fields
        private const string configurationsPath = @"Parameters\Pet";
        private Account _account;
        private bool _loaded;

        public PetBreederConfiguration(Account account)
        {
            _account = account;
            PetsToBreed = new ObservableCollection<PetsToBreedEntry>();
            Interval = 5;
        }

        // Properties
        public int Interval { get; set; }


        public ObservableCollection<PetsToBreedEntry> PetsToBreed { get; }

        private string ConfigFilePath => Path.Combine(configurationsPath,
            LanguageManager.Translate("68", _account.AccountConfig.Username, _account.Game.Character.Name));

       // public bool IsScriptPathValid => !string.IsNullOrEmpty(ScriptPath);


        public void Load()
        {
            _loaded = false;

            if (File.Exists(ConfigFilePath))
                using (var br = new BinaryReader(File.Open(ConfigFilePath, FileMode.Open, FileAccess.ReadWrite,
                    FileShare.ReadWrite)))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PetsToBreed.Clear();
                        var c = br.ReadByte();
                        for (var i = 0; i < c; i++)
                            PetsToBreed.Add(PetsToBreedEntry.Load(br));
                    });
                }
                
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
                    bw.Write((byte)PetsToBreed.Count);
                foreach (var obj in PetsToBreed)
                    obj.Save(bw);

            }
            
        }




        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // _scriptPath = null;
                _account = null;

                disposedValue = true;
            }
        }

        ~PetBreederConfiguration()
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

