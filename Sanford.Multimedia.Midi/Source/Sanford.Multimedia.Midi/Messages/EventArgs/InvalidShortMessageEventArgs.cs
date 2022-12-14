#region

using System;

#endregion

namespace Sanford.Multimedia.Midi;

public sealed class InvalidShortMessageEventArgs : EventArgs
{
    public InvalidShortMessageEventArgs(int message)
    {
        Message = message;
    }

    public int Message { get; }
}