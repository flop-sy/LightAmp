#region

using System.Runtime.InteropServices;

#endregion

namespace Sanford.Multimedia.Timers;

/// <summary>
///     Defines constants representing the timing format used by the Time struct.
/// </summary>
public enum TimeType
{
    Milliseconds = 0x0001,
    Samples = 0x0002,
    Bytes = 0x0004,
    Smpte = 0x0008,
    Midi = 0x0010,
    Ticks = 0x0020
}

/// <summary>
///     Represents the Windows Multimedia MMTIME structure.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct Time
{
    [FieldOffset(0)] public int type;

    [FieldOffset(4)] public int milliseconds;

    [FieldOffset(4)] public int samples;

    [FieldOffset(4)] public int byteCount;

    [FieldOffset(4)] public int ticks;

    //
    // SMPTE
    //

    [FieldOffset(4)] public byte hours;

    [FieldOffset(5)] public byte minutes;

    [FieldOffset(6)] public byte seconds;

    [FieldOffset(7)] public byte frames;

    [FieldOffset(8)] public byte framesPerSecond;

    [FieldOffset(9)] public byte dummy;

    [FieldOffset(10)] public byte pad1;

    [FieldOffset(11)] public byte pad2;

    //
    // MIDI
    //

    [FieldOffset(4)] public int songPositionPointer;
}