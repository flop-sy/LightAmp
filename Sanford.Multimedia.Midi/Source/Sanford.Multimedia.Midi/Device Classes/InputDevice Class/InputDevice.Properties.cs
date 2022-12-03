#region

using System;

#endregion

namespace Sanford.Multimedia.Midi
{
    public sealed partial class InputDevice
    {
        public override IntPtr Handle { get; }

        public int SysExBufferSize
        {
            get { return sysExBufferSize; }
            set
            {
                #region Require

                if (value < 1) throw new ArgumentOutOfRangeException();

                #endregion

                sysExBufferSize = value;
            }
        }

        public static int DeviceCount => midiInGetNumDevs();
    }
}