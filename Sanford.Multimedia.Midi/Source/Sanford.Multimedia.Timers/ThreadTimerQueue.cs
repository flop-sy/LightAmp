#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

#endregion

namespace Sanford.Multimedia.Timers;

/// <summary>
///     Queues and executes timer events in an internal worker thread.
/// </summary>
internal sealed class ThreadTimerQueue
{
    private static ThreadTimerQueue instance;
    private readonly List<Tick> tickQueue = new();
    private readonly Stopwatch watch = Stopwatch.StartNew();
    private Thread loop;

    private ThreadTimerQueue()
    {
    }

    public static ThreadTimerQueue Instance => instance ??= new ThreadTimerQueue();

    public void Add(ThreadTimer timer)
    {
        lock (this)
        {
            var tick = new Tick
            {
                Timer = timer,
                Time = watch.Elapsed
            };
            tickQueue.Add(tick);
            tickQueue.Sort();

            if (loop == null)
            {
                loop = new Thread(TimerLoop);
                loop.Start();
            }

            Monitor.PulseAll(this);
        }
    }

    public void Remove(ThreadTimer timer)
    {
        lock (this)
        {
            var i = 0;
            for (; i < tickQueue.Count; ++i)
                if (tickQueue[i].Timer == timer)
                    break;

            if (i < tickQueue.Count) tickQueue.RemoveAt(i);

            Monitor.PulseAll(this);
        }
    }

    private static TimeSpan Min(TimeSpan x0, TimeSpan x1)
    {
        return x0 > x1 ? x1 : x0;
    }

    /// <summary>
    ///     The thread to execute the timer events
    /// </summary>
    private void TimerLoop()
    {
        lock (this)
        {
            var maxTimeout = TimeSpan.FromMilliseconds(500);

            for (var queueEmptyCount = 0; queueEmptyCount < 3; ++queueEmptyCount)
            {
                var waitTime = maxTimeout;
                if (tickQueue.Count > 0)
                {
                    waitTime = Min(tickQueue[0].Time - watch.Elapsed, waitTime);
                    queueEmptyCount = 0;
                }

                if (waitTime > TimeSpan.Zero) Monitor.Wait(this, waitTime);

                if (tickQueue.Count <= 0) continue;

                var tick = tickQueue[0];
                var mode = tick.Timer.Mode;
                Monitor.Exit(this);
                tick.Timer.DoTick();
                Monitor.Enter(this);
                if (mode == TimerMode.Periodic)
                {
                    tick.Time += tick.Timer.PeriodTimeSpan;
                    tickQueue.Sort();
                }
                else
                {
                    tickQueue.RemoveAt(0);
                }
            }

            loop = null;
        }
    }

    private class Tick : IComparable
    {
        public TimeSpan Time;
        public ThreadTimer Timer;

        public int CompareTo(object obj)
        {
            if (obj is not Tick r) return -1;

            return Time.CompareTo(r.Time);
        }
    }
}