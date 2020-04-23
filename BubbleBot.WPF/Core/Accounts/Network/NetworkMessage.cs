using System;

namespace BubbleBot.Core.Accounts.Network
{
    public class NetworkMessage
    {
        // Constructor
        public NetworkMessage(string message, bool sent)
        {
            Message = message;
            Sent = sent;
            Time = DateTime.Now;
        }

        // Properties
        public string Message { get; }
        public bool Sent { get; }
        public DateTime Time { get; }
    }
}