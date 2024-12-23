using System;
using System.Timers;

namespace LazyMoon.Service
{
    public class BlazorTimerService
    {
        private System.Timers.Timer mTimer;

        public void SetTimer(double interval)
        {
            mTimer = new System.Timers.Timer(interval);
            mTimer.Elapsed += NotifyTimerElapsed;
            mTimer.Enabled = true;
        }

        public event Action OnElapsed;

        private void NotifyTimerElapsed(Object source, ElapsedEventArgs e)
        {
            OnElapsed?.Invoke();
        }
    }
}