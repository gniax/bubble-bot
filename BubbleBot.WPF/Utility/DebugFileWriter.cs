using System;
using System.IO;

namespace BubbleBot.Utility
{
    public static class DebugFileWriter
    {
        public static void WriteFile(string line, string path = "DebugFile.txt")
        {
            using (var sw = new StreamWriter(path, true))
            {
                sw.WriteLine("[" + DateTime.Now + "] " + line);
            }
        }

        public static void WriteFile(string[] lines, string path = "DebugFile.txt")
        {
            using (var sw = new StreamWriter(path, true))
            {
                foreach (var line in lines) sw.WriteLine("[" + DateTime.Now + "] " + line);
            }
        }
    }
}