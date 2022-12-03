#region

using System;
using System.ComponentModel;
using System.Diagnostics;

#endregion

namespace Sanford.Multimedia.Timers
{
    /// <summary>
    ///     Replacement for the Windows multimedia timer that also runs on Mono
    /// </summary>
    internal sealed class ThreadTimer : ITimer
    {
        private static readonly object[] emptyArgs = { EventArgs.Empty };
        private readonly ThreadTimerQueue queue;

        // Represents the method that raises the Tick event.
        private readonly EventRaiser tickRaiser;

        private bool disposed;

        private TimerMode mode;
        private TimeSpan period;
        private TimeSpan resolution;

        // For implementing IComponent.

        // The ISynchronizeInvoke object to use for marshaling events.
        private ISynchronizeInvoke synchronizingObject;

        public ThreadTimer()
            : this(ThreadTimerQueue.Instance)
        {
            if (!Stopwatch.IsHighResolution) throw new NotImplementedException("Stopwatch is not IsHighResolution");

            IsRunning = false;
            mode = TimerMode.Periodic;
            resolution = TimeSpan.FromMilliseconds(1);
            period = resolution;

            tickRaiser = OnTick;
        }

        private ThreadTimer(ThreadTimerQueue queue)
        {
            this.queue = queue;
        }

        public TimeSpan PeriodTimeSpan => period;

        public bool IsRunning { get; private set; }

        public TimerMode Mode
        {
            get
            {
                #region Require

                if (disposed) throw new ObjectDisposedException("Timer");

                #endregion

                return mode;
            }

            set
            {
                #region Require

                if (disposed) throw new ObjectDisposedException("Timer");

                #endregion

                mode = value;

                if (!IsRunning) return;

                Stop();
                Start();
            }
        }

        public int Period
        {
            get
            {
                #region Require

                if (disposed) throw new ObjectDisposedException("Timer");

                #endregion

                return (int)period.TotalMilliseconds;
            }
            set
            {
                #region Require

                if (disposed) throw new ObjectDisposedException("Timer");

                #endregion

                var wasRunning = IsRunning;

                if (wasRunning) Stop();

                period = TimeSpan.FromMilliseconds(value);

                if (wasRunning) Start();
            }
        }

        public int Resolution
        {
            get => (int)resolution.TotalMilliseconds;

            set => resolution = TimeSpan.FromMilliseconds(value);
        }

        public ISite Site { get; set; }

        /// <summary>
        ///     Gets or sets the object used to marshal event-handler calls.
        /// </summary>
        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                #region Require

                if (disposed) throw new ObjectDisposedException("Timer");

                #endregion

                return synchronizingObject;
            }
            set
            {
                #region Require

                if (disposed) throw new ObjectDisposedException("Timer");

                #endregion

                synchronizingObject = value;
            }
        }

        public event EventHandler Disposed;
        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler Tick;

        public void Dispose()
        {
            Stop();
            disposed = true;
            OnDisposed(EventArgs.Empty);
        }

        public void Start()
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("Timer");

            #endregion

            #region Guard

            if (IsRunning) return;

            #endregion

            // If the periodic event callback should be used.
            if (Mode == TimerMode.Periodic)
            {
                queue.Add(this);
                IsRunning = true;
            }
            // Else the one shot event callback should be used.
            else
            {
                throw new NotImplementedException();
            }

            if (SynchronizingObject != null && SynchronizingObject.InvokeRequired)
                SynchronizingObject.BeginInvoke(
                    new EventRaiser(OnStarted),
                    new object[] { EventArgs.Empty });
            else
                OnStarted(EventArgs.Empty);
        }

        public void Stop()
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("Timer");

            #endregion

            #region Guard

            if (!IsRunning) return;

            #endregion

            queue.Remove(this);
            IsRunning = false;

            if (SynchronizingObject != null && SynchronizingObject.InvokeRequired)
                SynchronizingObject.BeginInvoke(
                    new EventRaiser(OnStopped),
                    new object[] { EventArgs.Empty });
            else
                OnStopped(EventArgs.Empty);
        }

        internal void DoTick()
        {
            if (SynchronizingObject != null && SynchronizingObject.InvokeRequired)
                SynchronizingObject.BeginInvoke(tickRaiser, emptyArgs);
            else
                OnTick(EventArgs.Empty);
        }

        // Represents methods that raise events.
        private delegate void EventRaiser(EventArgs e);

        #region Event Raiser Methods

        // Raises the Disposed event.
        private void OnDisposed(EventArgs e)
        {
            var handler = Disposed;

            handler?.Invoke(this, e);
        }

        // Raises the Started event.
        private void OnStarted(EventArgs e)
        {
            var handler = Started;

            handler?.Invoke(this, e);
        }

        // Raises the Stopped event.
        private void OnStopped(EventArgs e)
        {
            var handler = Stopped;

            handler?.Invoke(this, e);
        }

        // Raises the Tick event.
        private void OnTick(EventArgs e)
        {
            var handler = Tick;

            handler?.Invoke(this, e);
        }

        #endregion
    }
}