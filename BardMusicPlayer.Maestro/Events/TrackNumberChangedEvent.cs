#region

using BardMusicPlayer.Seer;

#endregion

namespace BardMusicPlayer.Maestro.Events;

public sealed class TrackNumberChangedEvent : MaestroEvent
{
    internal TrackNumberChangedEvent(Game g, int trackNumber, bool isHost = false)
    {
        EventType = GetType();
        TrackNumber = trackNumber;
        game = g;
        IsHost = isHost;
    }

    public Game game { get; }
    public int TrackNumber { get; }
    public bool IsHost { get; }

    public override bool IsValid()
    {
        return true;
    }
}