using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Updates
{
    [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
    public class Updater
    {
        private readonly Dictionary<string, string> _downloadDestinations;
        private readonly List<string> _filesToDownload;

        // Fields
        private readonly WebClient _webClient;
        private int _currentFileIndex;
        private bool _updating;


        // Constructor
        public Updater(string baseUrl)
        {
            _webClient = new WebClient();
            _filesToDownload = new List<string>();
            _downloadDestinations = new Dictionary<string, string>();

            _webClient.BaseAddress = baseUrl;
            _webClient.DownloadProgressChanged += _webClient_DownloadProgressChanged;
            _webClient.DownloadFileCompleted += _webClient_DownloadFileCompleted;
        }


        // Events
        public event Action<int> UpdateStarted;
        public event Action<string> FileDownloadStarted;
        public event Action<int> FileDownloadProgress;
        public event Action UpdateFailed;
        public event Action UpdateFinished;


        public bool CheckForUpdates(Dictionary<string, string> upToDateFiles)
        {
            if (_updating)
                return false;

            // Check for temp files that need to be deleted
            foreach (var file in Directory.GetFiles(
                Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "*.bbtemp",
                SearchOption.AllDirectories)) File.Delete(file);

            _filesToDownload.Clear();
            _downloadDestinations.Clear();

            // If no update is needed, do no thing
            if (!IsUpdateNeeded(upToDateFiles))
                return false;

            StartUpdate();
            return true;
        }

        private void StartUpdate()
        {
            if (_updating)
                return;

            _updating = true;
            _currentFileIndex = -1;
            UpdateStarted?.Invoke(_filesToDownload.Count);

            ProcessUpdate();
        }

        private void ProcessUpdate()
        {
            _currentFileIndex++;

            // If we downloaded everything
            if (_currentFileIndex >= _filesToDownload.Count)
            {
                FinishUpdate();
            }
            // Otherwise
            else
            {
                var fileToDownload = _filesToDownload[_currentFileIndex];
                var destPath = Path.GetTempFileName();
                _downloadDestinations.Add(fileToDownload, destPath);
                _webClient.DownloadFileAsync(new Uri(fileToDownload, UriKind.Relative), destPath);
                FileDownloadStarted?.Invoke(fileToDownload);
            }
        }

        private void FinishUpdate()
        {
            if (!_updating)
                return;

            var i = 0;

            // Key: original file name
            // Value: temp destination of downloaded file
            foreach (var kvp in _downloadDestinations)
            {
                // If the file we want to move to the current directory already exists, rename it to a tmp name
                if (File.Exists(kvp.Key))
                {
                    File.Move(kvp.Key, $"temp{i}.bbtemp");
                    i++;
                }

                var directoryPath = Path.GetDirectoryName(kvp.Key);
                if (!string.IsNullOrEmpty(directoryPath)) Directory.CreateDirectory(Path.GetDirectoryName(kvp.Key));

                File.Move(kvp.Value, kvp.Key);
            }

            UpdateFinished?.Invoke();
        }

        private void _webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!_updating)
                return;

            FileDownloadProgress?.Invoke(e.ProgressPercentage);
        }

        private async void _webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (!_updating)
                return;

            if (e.Error != null || e.Cancelled)
            {
                UpdateFailed?.Invoke();
                return;
            }

            await Task.Delay(1000);
            ProcessUpdate();
        }

        private bool IsUpdateNeeded(Dictionary<string, string> upToDateFiles)
        {
            foreach (var kvp in upToDateFiles)
                if (File.Exists(kvp.Key))
                {
                    // File to update
                    if (kvp.Value != GetSha512HashFromFile(kvp.Key)) _filesToDownload.Add(kvp.Key);
                }
                else
                {
                    // File to download
                    _filesToDownload.Add(kvp.Key);
                }

            return _filesToDownload.Count > 0;
        }

        private static string GetSha512HashFromFile(string fileName)
        {
            var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            SHA512 sha512 = new SHA512CryptoServiceProvider();
            var byteHash = sha512.ComputeHash(file);
            file.Close();

            var hashString = new StringBuilder();
            for (var i = 0; i < byteHash.Length; i++)
                hashString.Append(byteHash[i].ToString("x2"));

            return hashString.ToString();
        }
    }
}