namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;

internal sealed class ChatLogPointers
{
    public uint LineCount { get; set; }

    public long LogEnd { get; set; }

    public long LogNext { get; set; }

    public long LogStart { get; set; }

    public long OffsetArrayEnd { get; set; }

    public long OffsetArrayPos { get; set; }

    public long OffsetArrayStart { get; set; }
}