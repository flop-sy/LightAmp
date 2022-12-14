namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core.Enums;

internal class Performance
{
    public enum Status : byte
    {
        Closed,
        Loading,
        Opened,
        SwitchingNote,
        HoldingNote
    }
}