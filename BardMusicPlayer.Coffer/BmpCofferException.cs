#region

using System;
using BardMusicPlayer.Quotidian;

#endregion

namespace BardMusicPlayer.Coffer;

public sealed class BmpCofferException : BmpException
{
    public BmpCofferException(string message) : base(message)
    {
    }

    public BmpCofferException(string message, Exception inner) : base(message, inner)
    {
    }
}