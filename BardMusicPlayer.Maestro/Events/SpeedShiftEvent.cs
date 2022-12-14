#region

using BardMusicPlayer.Seer;

#endregion

namespace BardMusicPlayer.Maestro.Events;

public sealed class SpeedShiftEvent : MaestroEvent
{
    internal SpeedShiftEvent(Game g, float speedShift, bool isHost = false)
    {
        EventType = GetType();
        SpeedShift = speedShift;
        game = g;
        IsHost = isHost;
    }

    public Game game { get; }
    public float SpeedShift { get; }
    public bool IsHost { get; }

    public override bool IsValid()
    {
        return true;
    }
}