#region

using System;

#endregion

namespace Sanford.Multimedia.Midi;

/// <summary>
/// </summary>
public sealed class NoOpEventArgs : EventArgs
{
    public NoOpEventArgs(int data)
    {
        Data = data;
    }

    public int Data { get; }
}