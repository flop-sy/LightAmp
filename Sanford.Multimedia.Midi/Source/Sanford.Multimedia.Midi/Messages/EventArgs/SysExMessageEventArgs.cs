#region

using System;

#endregion

namespace Sanford.Multimedia.Midi;

public sealed class SysExMessageEventArgs : EventArgs
{
    public SysExMessageEventArgs(Track track, SysExMessage message)
    {
        Message = message;
        MidiTrack = track;
    }

    public SysExMessage Message { get; }

    public Track MidiTrack { get; }
}