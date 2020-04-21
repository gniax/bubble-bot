using System;
using System.IO;
using System.Linq;

namespace BubbleBot.Server.Utility.Extensions
{
    public static class FileWriter
    {
        public static void BasicWriteMessage(string line, string path = "default.txt")
        {
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine("[" + DateTime.Now.ToString() + "] " + line);
            }
        }
        public static void BasicWriteMessage(string[] lines, string path = "default.txt")
        {
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                foreach (var line in lines)
                {
                    sw.WriteLine("[" + DateTime.Now.ToString() + "] " + line);
                }
            }
        }
        public static void EmptyWriteMessage(string line, string path = "default.txt")
        {
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(line);
            }
        }
        public static void EmptyWriteMessage(string[] lines, string path = "default.txt")
        {
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }
        public static bool FileContainsWords(string[] words, string path = "default.txt")
        {
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (!string.IsNullOrEmpty(line) && words.All(line.Contains))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                Console.WriteLine("Path File {0} not found!", path);
                return false;
            }
        }
        public static bool FileContainsWord(string word, string path = "default.txt")
        {
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                for (var i = 0; i < lines.Length; i += 1)
                {
                    var line = lines[i];
                    if (!string.IsNullOrEmpty(line) && line.Contains(word))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                Console.WriteLine("Path File {0} not found!", path);
                return false;
            }
        }

    }
}
