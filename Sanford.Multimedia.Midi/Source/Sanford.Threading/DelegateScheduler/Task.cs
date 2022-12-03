#region

using System;
using System.Diagnostics;

#endregion

namespace Sanford.Threading
{
    public sealed class Task : IComparable
    {
        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (!(obj is Task t)) throw new ArgumentException("obj is not the same type as this instance.");

            return -nextTimeout.CompareTo(t.nextTimeout);
        }

        #endregion

        #region Task Members

        #region Fields

        // The number of times left to invoke the delegate associated with this Task.

        // The interval between delegate invocation.

        // The delegate to invoke.

        // The arguments to pass to the delegate when it is invoked.
        private readonly object[] args;

        // The time for the next timeout;
        private DateTime nextTimeout;

        // For locking.
        private readonly object lockObject = new object();

        #endregion

        #region Construction

        internal Task(
            int count,
            int millisecondsTimeout,
            Delegate method,
            object[] args)
        {
            Count = count;
            MillisecondsTimeout = millisecondsTimeout;
            Method = method;
            this.args = args;

            ResetNextTimeout();
        }

        #endregion

        #region Methods

        internal void ResetNextTimeout()
        {
            nextTimeout = DateTime.Now.AddMilliseconds(MillisecondsTimeout);
        }

        internal object Invoke(DateTime signalTime)
        {
            Debug.Assert(Count == DelegateScheduler.Infinite || Count > 0);

            var returnValue = Method.DynamicInvoke(args);

            if (Count == DelegateScheduler.Infinite)
            {
                nextTimeout = nextTimeout.AddMilliseconds(MillisecondsTimeout);
            }
            else
            {
                Count--;

                if (Count > 0) nextTimeout = nextTimeout.AddMilliseconds(MillisecondsTimeout);
            }

            return returnValue;
        }

        public object[] GetArgs()
        {
            return args;
        }

        #endregion

        #region Properties

        public DateTime NextTimeout => nextTimeout;

        public int Count { get; private set; }

        public Delegate Method { get; }

        public int MillisecondsTimeout { get; }

        #endregion

        #endregion
    }
}