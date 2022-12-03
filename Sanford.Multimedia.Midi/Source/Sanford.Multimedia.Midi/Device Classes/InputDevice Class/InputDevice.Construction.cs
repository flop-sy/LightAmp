#region

using System;
using System.Diagnostics;
using Sanford.Threading;

#endregion

namespace Sanford.Multimedia.Midi
{
    public sealed partial class InputDevice
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the InputDevice class with the
        ///     specified device ID.
        /// </summary>
        public InputDevice(int deviceID, bool postEventsOnCreationContext = true,
            bool postDriverCallbackToDelegateQueue = true)
            : base(deviceID)
        {
            midiInProc = HandleMessage;

            delegateQueue = new DelegateQueue();
            var result = midiInOpen(out var intPtr, deviceID, midiInProc, IntPtr.Zero, CALLBACK_FUNCTION);

            Debug.WriteLine("MidiIn handle:" + intPtr.ToInt64());

            if (result != DeviceException.MMSYSERR_NOERROR) throw new InputDeviceException(result);

            PostEventsOnCreationContext = postEventsOnCreationContext;
            PostDriverCallbackToDelegateQueue = postDriverCallbackToDelegateQueue;
        }

        ~InputDevice()
        {
            if (IsDisposed) return;

            midiInReset(Handle);
            midiInClose(Handle);
        }

        #endregion
    }
}