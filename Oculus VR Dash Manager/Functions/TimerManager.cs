using System;
using System.Collections.Generic;
using System.Timers;

namespace OVR_Dash_Manager.Functions
{
    public static class TimerManager
    {
        private static readonly object TimerLock = new object();
        private static Dictionary<string, Timer> Timers = new Dictionary<string, Timer>();

        public static bool SetNewInterval(string timerID, TimeSpan interval)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));

            lock (TimerLock)
            {
                if (Timers.ContainsKey(timerID))
                {
                    var timer = Timers[timerID];
                    timer.Interval = interval.TotalMilliseconds;
                    return true;
                }
            }

            return false;
        }

        public static bool CreateTimer(string timerID, TimeSpan interval, ElapsedEventHandler tickHandler, bool repeat = true)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));
            if (tickHandler == null) throw new ArgumentNullException(nameof(tickHandler));

            lock (TimerLock)
            {
                if (!Timers.ContainsKey(timerID))
                {
                    var timer = new Timer
                    {
                        Interval = interval.TotalMilliseconds,
                        AutoReset = repeat,
                        Enabled = false
                    };

                    timer.Elapsed += tickHandler;
                    Timers.Add(timerID, timer);

                    return true;
                }
            }

            return false;
        }

        public static bool StartTimer(string timerID)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));

            lock (TimerLock)
            {
                if (Timers.ContainsKey(timerID))
                {
                    var timer = Timers[timerID];
                    timer.Start();
                    return true;
                }
            }

            return false;
        }

        public static bool StopTimer(string timerID)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));

            lock (TimerLock)
            {
                if (Timers.ContainsKey(timerID))
                {
                    var timer = Timers[timerID];
                    timer.Stop();
                    return true;
                }
            }

            return false;
        }

        public static bool TimerExists(string timerID)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));

            lock (TimerLock)
                return Timers.ContainsKey(timerID);
        }

        public static void DisposeTimer(string timerID)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));

            lock (TimerLock)
            {
                if (Timers.ContainsKey(timerID))
                {
                    var timer = Timers[timerID];
                    Timers.Remove(timerID);

                    timer.Stop();
                    timer.Dispose();
                }
            }
        }

        public static void DisposeAllTimers()
        {
            lock (TimerLock)
            {
                foreach (var timer in Timers.Values)
                {
                    timer.Stop();
                    timer.Dispose();
                }

                Timers.Clear();
            }
        }
    }
}