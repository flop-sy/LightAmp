#region

using System;
using System.Threading;

#endregion

namespace Sanford.Multimedia
{
    public abstract class Device : IDisposable
    {
        protected const int CALLBACK_FUNCTION = 0x30000;

        protected const int CALLBACK_EVENT = 0x50000;

        protected SynchronizationContext context;

        protected Device(int deviceID)
        {
            DeviceID = deviceID;

            context = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        /// <summary>
        ///     Gets the device handle.
        /// </summary>
        public abstract IntPtr Handle { get; }

        public int DeviceID { get; }

        public bool IsDisposed { get; private set; }

        #region IDisposable

        /// <summary>
        ///     Disposes of the device.
        /// </summary>
        public abstract void Dispose();

        #endregion

        // Indicates whether the device has been disposed.

        public event EventHandler<ErrorEventArgs> Error;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            IsDisposed = true;

            GC.SuppressFinalize(this);
        }

        protected virtual void OnError(ErrorEventArgs e)
        {
            var handler = Error;

            if (handler != null)
                context.Post(delegate { handler(this, e); }, null);
        }

        /// <summary>
        ///     Closes the MIDI device.
        /// </summary>
        public abstract void Close();

        /// <summary>
        ///     Resets the device.
        /// </summary>
        public abstract void Reset();
    }
}