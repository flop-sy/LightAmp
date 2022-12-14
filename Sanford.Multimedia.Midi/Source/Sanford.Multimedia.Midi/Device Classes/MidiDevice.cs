#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace Sanford.Multimedia.Midi;

/// <summary>
///     The base class for all MIDI devices.
/// </summary>
public abstract class MidiDevice : Device
{
    #region MidiDevice Members

    #region Win32 Midi Device Functions

    [DllImport("winmm.dll")]
    private static extern int midiConnect(IntPtr handleA, IntPtr handleB, IntPtr reserved);

    [DllImport("winmm.dll")]
    private static extern int midiDisconnect(IntPtr handleA, IntPtr handleB, IntPtr reserved);

    #endregion

    // Size of the MidiHeader structure.
    protected static readonly int SizeOfMidiHeader;

    static MidiDevice()
    {
        SizeOfMidiHeader = Marshal.SizeOf(typeof(MidiHeader));
    }

    protected MidiDevice(int deviceID) : base(deviceID)
    {
    }

    /// <summary>
    ///     Connects a MIDI InputDevice to a MIDI thru or OutputDevice, or
    ///     connects a MIDI thru device to a MIDI OutputDevice.
    /// </summary>
    /// <param name="handleA">
    ///     Handle to a MIDI InputDevice or a MIDI thru device (for thru
    ///     devices, this handle must belong to a MIDI OutputDevice).
    /// </param>
    /// <param name="handleB">
    ///     Handle to the MIDI OutputDevice or thru device.
    /// </param>
    /// <exception cref="DeviceException">
    ///     If an error occurred while connecting the two devices.
    /// </exception>
    public static void Connect(IntPtr handleA, IntPtr handleB)
    {
        var result = midiConnect(handleA, handleB, IntPtr.Zero);

        if (result != DeviceException.MMSYSERR_NOERROR) throw new MidiDeviceException(result);
    }

    /// <summary>
    ///     Disconnects a MIDI InputDevice from a MIDI thru or OutputDevice, or
    ///     disconnects a MIDI thru device from a MIDI OutputDevice.
    /// </summary>
    /// <param name="handleA">
    ///     Handle to a MIDI InputDevice or a MIDI thru device.
    /// </param>
    /// <param name="handleB">
    ///     Handle to the MIDI OutputDevice to be disconnected.
    /// </param>
    /// <exception cref="DeviceException">
    ///     If an error occurred while disconnecting the two devices.
    /// </exception>
    public static void Disconnect(IntPtr handleA, IntPtr handleB)
    {
        var result = midiDisconnect(handleA, handleB, IntPtr.Zero);

        if (result != DeviceException.MMSYSERR_NOERROR) throw new MidiDeviceException(result);
    }

    #endregion
}