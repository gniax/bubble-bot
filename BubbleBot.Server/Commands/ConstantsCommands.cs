using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BubbleBot.Server.Messages;
using BubbleBot.Server.Utility;

namespace BubbleBot.Server.Commands
{
    public static class ConstantsCommands
    {

        [Command("refreshFilesHashes")]
        public static void RefreshFilesHashesCommand(string[] args)
        {
            string releaseDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Release");

            var filesHashes = new Dictionary<string, string>();

            foreach (var file in Directory.GetFiles(releaseDir, "*.*", SearchOption.AllDirectories))
            {
                string filePath = file.Replace(releaseDir, "").Replace(@"\", @"/").Substring(1);
                filesHashes.Add(filePath, GetSha512HashFromFile(file));
            }

            Constants.FilesHashes = filesHashes;
            if (args.Length != 0)
                Console.WriteLine("Files hashes refreshed !");
        }

        [Command("setDTVersions")]
        public static void SetDTVersionsCommand(string[] args)
        {
            if (args.Length != 0)
                return;

            if (SetVersions.setVersions())
            {
                ServerMain.BroadcastMessage(new DTVersionsMessage(Constants.AppVersion, Constants.BuildVersion, Constants.AssetsVersion, Constants.StaticDataVersion), true);
                Console.WriteLine("DT Versions updated and broadcasted.");
            }
            else
            {
                Console.WriteLine("DT Versions update failed - try again");
            }
        }

        private static string GetSha512HashFromFile(string fileName)
        {
            var file = new FileStream(fileName, FileMode.Open);
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
