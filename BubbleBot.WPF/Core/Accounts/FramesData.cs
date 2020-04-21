using System.Collections.Generic;

namespace BubbleBot.Core.Accounts
{
    public class FramesData : IClearable
    {
        // Constructor
        public FramesData()
        {
            Clear();
        }

        // Properties
        public uint Sequence { get; set; }
        public int CaptchasCounter { get; set; }
        public List<sbyte> Key { get; set; }
        public string Salt { get; set; }
        public string Ticket { get; set; }
        public bool Initialized { get; set; }
        public uint ServerToAutoConnectTo { get; set; }


        public void Clear()
        {
            Sequence = 0;
            CaptchasCounter = 0;
            Key = null;
            Salt = null;
            Ticket = null;
            Initialized = false;
            ServerToAutoConnectTo = 0;
        }
    }
}