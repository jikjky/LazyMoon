using System;
using System.Timers;

namespace LazyMoon.Class.Service
{
    public class BlazorTimerService
    {
        private System.Timers.Timer? mTimer;

        public void SetTimer(double interval)
        {
            mTimer = new System.Timers.Timer(interval);
            mTimer.Elapsed += (o, e) => { OnElapsed?.Invoke(); };
            mTimer.Enabled = true;
        }

        public event Action? OnElapsed;
    }
}