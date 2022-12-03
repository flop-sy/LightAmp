#region

using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    ///     Represents a MIDI device capable of receiving MIDI events.
    /// </summary>
    public sealed partial class InputDevice : MidiDevice
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (lockObject)
                {
                    Reset();

                    var result = midiInClose(Handle);

                    if (result == DeviceException.MMSYSERR_NOERROR)
                        delegateQueue.Dispose();
                    else
                        throw new InputDeviceException(result);
                }
            }
            else
            {
                midiInReset(Handle);
                midiInClose(Handle);
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    ///     The exception that is thrown when a error occurs with the InputDevice
    ///     class.
    /// </summary>
    public sealed class InputDeviceException : MidiDeviceException
    {
        #region InputDeviceException Members

        #region Win32 Midi Input Error Function

        [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
        private static extern int midiInGetErrorText(int errCode,
            StringBuilder errMsg, int sizeOfErrMsg);

        #endregion

        #region Fields

        // Error message.
        private readonly StringBuilder errMsg = new StringBuilder(128);

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the InputDeviceException class with
        ///     the specified error code.
        /// </summary>
        /// <param name="errCode">
        ///     The error code.
        /// </param>
        public InputDeviceException(int errCode) : base(errCode)
        {
            // Get error message.
            midiInGetErrorText(errCode, errMsg, errMsg.Capacity);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a message that describes the current exception.
        /// </summary>
        public override string Message => errMsg.ToString();

        #endregion

        #endregion
    }
}