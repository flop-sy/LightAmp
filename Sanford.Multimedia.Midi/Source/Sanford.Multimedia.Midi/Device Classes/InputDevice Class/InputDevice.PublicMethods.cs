#region

using System;
using System.Threading;

#endregion

namespace Sanford.Multimedia.Midi;

public sealed partial class InputDevice
{
    public override void Close()
    {
        #region Guard

        if (IsDisposed) return;

        #endregion

        Dispose(true);
    }

    public void StartRecording()
    {
        #region Require

        if (IsDisposed) throw new ObjectDisposedException("InputDevice");

        #endregion

        #region Guard

        if (recording) return;

        #endregion

        lock (lockObject)
        {
            var result = AddSysExBuffer();

            if (result == DeviceException.MMSYSERR_NOERROR) result = AddSysExBuffer();

            if (result == DeviceException.MMSYSERR_NOERROR) result = AddSysExBuffer();

            if (result == DeviceException.MMSYSERR_NOERROR) result = AddSysExBuffer();

            if (result == DeviceException.MMSYSERR_NOERROR) result = midiInStart(Handle);

            if (result == DeviceException.MMSYSERR_NOERROR)
                recording = true;
            else
                throw new InputDeviceException(result);
        }
    }

    public void StopRecording()
    {
        #region Require

        if (IsDisposed) throw new ObjectDisposedException("InputDevice");

        #endregion

        #region Guard

        if (!recording) return;

        #endregion

        lock (lockObject)
        {
            var result = midiInStop(Handle);

            if (result == DeviceException.MMSYSERR_NOERROR)
                recording = false;
            else
                throw new InputDeviceException(result);
        }
    }

    public override void Reset()
    {
        #region Require

        if (IsDisposed) throw new ObjectDisposedException("InputDevice");

        #endregion

        lock (lockObject)
        {
            resetting = true;

            var result = midiInReset(Handle);

            if (result == DeviceException.MMSYSERR_NOERROR)
            {
                recording = false;

                while (bufferCount > 0) Monitor.Wait(lockObject);

                resetting = false;
            }
            else
            {
                resetting = false;

                throw new InputDeviceException(result);
            }
        }
    }

    public static MidiInCaps GetDeviceCapabilities(int deviceID)
    {
        var caps = new MidiInCaps();

        var devID = (IntPtr)deviceID;
        var result = midiInGetDevCaps(devID, ref caps, SizeOfMidiHeader);

        if (result != DeviceException.MMSYSERR_NOERROR) throw new InputDeviceException(result);

        return caps;
    }

    public override void Dispose()
    {
        #region Guard

        if (IsDisposed) return;

        #endregion

        Dispose(true);
    }
}