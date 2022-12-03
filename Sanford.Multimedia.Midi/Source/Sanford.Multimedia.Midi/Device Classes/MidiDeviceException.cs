namespace Sanford.Multimedia.Midi
{
    /// <summary>
    ///     The base class for all MIDI device exception classes.
    /// </summary>
    public class MidiDeviceException : DeviceException
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the DeviceException class with the
        ///     specified error code.
        /// </summary>
        /// <param name="errCode">
        ///     The error code.
        /// </param>
        public MidiDeviceException(int errCode) : base(errCode)
        {
        }

        #endregion

        #region Error Codes

        public const int MIDIERR_UNPREPARED = 64; /* header not prepared */
        public const int MIDIERR_STILLPLAYING = 65; /* still something playing */
        public const int MIDIERR_NOMAP = 66; /* no configured instruments */
        public const int MIDIERR_NOTREADY = 67; /* hardware is still busy */
        public const int MIDIERR_NODEVICE = 68; /* port no longer connected */
        public const int MIDIERR_INVALIDSETUP = 69; /* invalid MIF */
        public const int MIDIERR_BADOPENMODE = 70; /* operation unsupported w/ open mode */
        public const int MIDIERR_DONT_CONTINUE = 71; /* thru device 'eating' a message */
        public const int MIDIERR_LASTERROR = 71; /* last error in range */

        #endregion
    }
}