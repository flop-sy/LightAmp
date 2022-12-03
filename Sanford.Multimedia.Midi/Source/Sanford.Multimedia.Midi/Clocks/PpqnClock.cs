#region

using System;

#endregion

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    ///     Provides basic functionality for generating tick events with pulses per
    ///     quarter note resolution.
    /// </summary>
    public abstract class PpqnClock : IClock
    {
        #region PpqnClock Members

        #region Fields

        /// <summary>
        ///     The default tempo in microseconds: 120bpm.
        /// </summary>
        public const int DefaultTempo = 500000;

        /// <summary>
        ///     The minimum pulses per quarter note value.
        /// </summary>
        public const int PpqnMinValue = 24;

        // The number of microseconds per millisecond.
        private const int MicrosecondsPerMillisecond = 1000;

        // The pulses per quarter note value.
        private int ppqn = PpqnMinValue;

        // The tempo in microseconds.
        private int tempo = DefaultTempo;

        private float tempoSpeed = 1.0f;

        // The product of the timer period, the pulses per quarter note, and
        // the number of microseconds per millisecond.
        private int periodResolution;

        // The number of ticks per MIDI clock.

        // The running fractional tick count.
        private int fractionalTicks;

        // The timer period.
        private readonly int timerPeriod;

        // Indicates whether the clock is running.
        protected bool running = false;

        #endregion

        #region Construction

        protected PpqnClock(int timerPeriod)
        {
            #region Require

            if (timerPeriod < 1)
                throw new ArgumentOutOfRangeException(nameof(timerPeriod), timerPeriod,
                    "Timer period cannot be less than one.");

            #endregion

            this.timerPeriod = timerPeriod;

            CalculatePeriodResolution();
            CalculateTicksPerClock();
        }

        #endregion

        #region Methods

        protected int GetTempo()
        {
            return tempo;
        }

        protected void SetTempo(int tempo)
        {
            #region Require

            if (tempo < 1)
                throw new ArgumentOutOfRangeException(
                    "Tempo out of range.");

            #endregion

            this.tempo = tempo;
        }

        protected float GetTempoSpeed()
        {
            return tempoSpeed;
        }

        protected void SetTempoSpeed(float speed)
        {
            #region Require

            if (speed < 0.1f || speed > 5.0f)
                throw new ArgumentOutOfRangeException(
                    "Tempo speed out of range.");

            #endregion

            tempoSpeed = speed;
        }

        protected void Reset()
        {
            fractionalTicks = 0;
        }

        protected int GenerateTicks()
        {
            var t = (int)(tempo / tempoSpeed);
            var ticks = (fractionalTicks + periodResolution) / t;
            fractionalTicks += periodResolution - ticks * t;

            return ticks;
        }

        private void CalculatePeriodResolution()
        {
            periodResolution = ppqn * timerPeriod * MicrosecondsPerMillisecond;
        }

        private void CalculateTicksPerClock()
        {
            TicksPerClock = ppqn / PpqnMinValue;
        }

        protected virtual void OnTick(EventArgs e)
        {
            var handler = Tick;

            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStarted(EventArgs e)
        {
            var handler = Started;

            handler?.Invoke(this, e);
        }

        protected virtual void OnStopped(EventArgs e)
        {
            var handler = Stopped;

            handler?.Invoke(this, e);
        }

        protected virtual void OnContinued(EventArgs e)
        {
            var handler = Continued;

            handler?.Invoke(this, e);
        }

        #endregion

        #region Properties

        public int Ppqn
        {
            get { return ppqn; }
            set
            {
                #region Require

                if (value < PpqnMinValue)
                    throw new ArgumentOutOfRangeException("Ppqn", value,
                        "Pulses per quarter note is smaller than 24.");

                #endregion

                ppqn = value;

                CalculatePeriodResolution();
                CalculateTicksPerClock();
            }
        }

        public abstract int Ticks { get; }

        public int TicksPerClock { get; private set; }

        #endregion

        #endregion

        #region IClock Members

        public event EventHandler Tick;

        public event EventHandler Started;

        public event EventHandler Continued;

        public event EventHandler Stopped;

        public bool IsRunning => running;

        #endregion
    }
}