using System;
using System.Collections.Generic;
using System.Text;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Core.Logs
{
    public class LogMessage
    {
        // Constructor
        public LogMessage(string source, string message, string color, List<ObjectItem> objectitems = null)
        {
            Source = source;
            Message = message;
            Color = color;
            Time = DateTime.Now;
            ObjectItems = objectitems;
        }

        // Properties
        public string Source { get; }
        public string Color { get; }
        public string Message { get; set; }
        public DateTime Time { get; }
        public List<ObjectItem> ObjectItems { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var source = string.IsNullOrEmpty(Source) ? "" : $" [{Source}]";
            sb.Append($"[{Time:HH:mm:ss:ffff}]{source} {Message}");

            return sb.ToString();
        }
    }
}