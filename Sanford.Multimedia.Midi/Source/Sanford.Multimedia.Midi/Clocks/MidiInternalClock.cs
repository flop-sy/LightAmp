#region

using System;
using System.ComponentModel;
using System.Threading;
using Sanford.Multimedia.Timers;

#endregion

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    ///     Generates clock events internally.
    /// </summary>
    public sealed class MidiInternalClock : PpqnClock, IComponent
    {
        #region IDisposable Members

        public void Dispose()
        {
            #region Guard

            if (disposed) return;

            #endregion

            if (running)
                // Stop the multimedia timer.
                timer.Stop();

            disposed = true;

            timer.Dispose();

            GC.SuppressFinalize(this);

            OnDisposed(EventArgs.Empty);
        }

        #endregion

        #region MidiInternalClock Members

        #region Fields

        // Used for generating tick events.
        private readonly ITimer timer;

        // Parses meta message tempo change messages.
        private TempoChangeBuilder builder = new TempoChangeBuilder();

        // Tick accumulator.
        private int ticks;

        // Indicates whether the clock has been disposed.
        private bool disposed;

        private int sleep;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the MidiInternalClock class.
        /// </summary>
        public MidiInternalClock()
            : this(TimerCaps.Default.periodMin)
        {
        }

        public MidiInternalClock(int timerPeriod) : base(timerPeriod)
        {
            timer = TimerFactory.Create();
            timer.Period = timerPeriod;
            timer.Tick += HandleTick;
        }

        /// <summary>
        ///     Initializes a new instance of the MidiInternalClock class with the
        ///     specified IContainer.
        /// </summary>
        /// <param name="container">
        ///     The IContainer to which the MidiInternalClock will add itself.
        /// </param>
        public MidiInternalClock(IContainer container) :
            this()
        {
            ///
            /// Required for Windows.Forms Class Composition Designer support
            ///
            container.Add(this);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Sleeps for a certain time
        /// </summary>
        public void Sleep(int ms)
        {
            sleep += ms;
        }

        /// <summary>
        ///     Starts the MidiInternalClock.
        /// </summary>
        public void Start()
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("MidiInternalClock");

            #endregion

            #region Guard

            if (running) return;

            #endregion

            ticks = 0;

            Reset();

            OnStarted(EventArgs.Empty);

            // Start the multimedia timer in order to start generating ticks.
            timer.Start();

            // Indicate that the clock is now running.
            running = true;
        }

        /// <summary>
        ///     Resumes tick generation from the current position.
        /// </summary>
        public void Continue()
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("MidiInternalClock");

            #endregion

            #region Guard

            if (running) return;

            #endregion

            // Raise Continued event.
            OnContinued(EventArgs.Empty);

            // Start multimedia timer in order to start generating ticks.
            timer.Start();

            // Indicate that the clock is now running.
            running = true;
        }

        /// <summary>
        ///     Stops the MidiInternalClock.
        /// </summary>
        public void Stop()
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("MidiInternalClock");

            #endregion

            #region Guard

            if (!running) return;

            #endregion

            // Stop the multimedia timer.
            timer.Stop();

            // Indicate that the clock is not running.
            running = false;

            OnStopped(EventArgs.Empty);
        }

        public void SetTicks(int ticks)
        {
            #region Require

            if (ticks < 0) throw new ArgumentOutOfRangeException();

            #endregion

            if (IsRunning) Stop();

            this.ticks = ticks;

            Reset();
        }

        public void Process(MetaMessage message)
        {
            #region Require

            if (message == null) throw new ArgumentNullException(nameof(message));

            #endregion

            #region Guard

            if (message.MetaType != MetaType.Tempo) return;

            #endregion

            var builder = new TempoChangeBuilder(message);

            // Set the new tempo.
            Tempo = builder.Tempo;
        }

        #region Event Raiser Methods

        private void OnDisposed(EventArgs e)
        {
            var handler = Disposed;

            handler?.Invoke(this, e);
        }

        #endregion

        #region Event Handler Methods

        // Handles Tick events generated by the multimedia timer.
        private void HandleTick(object sender, EventArgs e)
        {
            if (sleep != 0)
            {
                Thread.Sleep(sleep);
                sleep = 0;
            }

            var t = GenerateTicks();

            for (var i = 0; i < t; i++)
            {
                OnTick(EventArgs.Empty);

                ticks++;
            }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the tempo speed multiplier.
        /// </summary>
        public float TempoSpeed
        {
            get => GetTempoSpeed();
            set => SetTempoSpeed(value);
        }

        /// <summary>
        ///     Gets or sets the tempo in microseconds per beat.
        /// </summary>
        public int Tempo
        {
            get
            {
                #region Require

                if (disposed) throw new ObjectDisposedException("MidiInternalClock");

                #endregion

                return GetTempo();
            }
            set
            {
                #region Require

                if (disposed) throw new ObjectDisposedException("MidiInternalClock");

                #endregion

                SetTempo(value);
            }
        }

        public override int Ticks => ticks;

        #endregion

        #endregion

        #region IComponent Members

        public event EventHandler Disposed;

        public ISite Site { get; set; }

        #endregion
    }
}