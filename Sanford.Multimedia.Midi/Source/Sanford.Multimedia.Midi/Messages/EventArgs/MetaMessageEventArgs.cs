#region

using System;

#endregion

namespace Sanford.Multimedia.Midi;

public sealed class MetaMessageEventArgs : EventArgs
{
    public MetaMessageEventArgs(Track track, MetaMessage message)
    {
        Message = message;
        MidiTrack = track;
    }

    public MetaMessage Message { get; }

    public Track MidiTrack { get; }
}