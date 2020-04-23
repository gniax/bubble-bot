using System;
using System.Threading;

namespace BubbleBot.Utility
{
    public class TimerWrapper : IDisposable
    {
        // Fields
        private Timer _timer;


        // Constructor
        public TimerWrapper(int interval, TimerCallback callback)
        {
            Interval = interval;
            _timer = new Timer(callback, null, Timeout.Infinite, Timeout.Infinite);
        }


        // Properties
        public bool Enabled { get; private set; }
        public int Interval { get; set; }

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }


        public void Start(bool rightAway = false)
        {
            if (Enabled)
                return;

            Enabled = true;
            _timer.Change(rightAway ? 0 : Interval, Interval);
        }

        public void Stop()
        {
            if (!Enabled)
                return;

            Enabled = false;
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}