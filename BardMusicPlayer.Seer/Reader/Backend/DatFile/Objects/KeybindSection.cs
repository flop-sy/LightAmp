#region

using System;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects;

internal sealed class KeybindSection : IDisposable
{
    public byte Type { get; set; }

    public int Size { get; set; }

    public byte[] Data { get; set; }

    public void Dispose()
    {
        Data = null;
    }

    ~KeybindSection()
    {
        Dispose();
    }
}