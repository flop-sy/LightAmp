#region

using System;

#endregion

namespace Sanford.Multimedia.Midi
{
    public sealed class ChannelMessageEventArgs : EventArgs
    {
        public ChannelMessageEventArgs(Track track, ChannelMessage message)
        {
            Message = message;
            MidiTrack = track;
        }

        public ChannelMessage Message { get; }

        public Track MidiTrack { get; }
    }
}