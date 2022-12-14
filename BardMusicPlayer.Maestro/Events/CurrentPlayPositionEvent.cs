#region

using System;

#endregion

namespace BardMusicPlayer.Maestro.Events;

public sealed class CurrentPlayPositionEvent : MaestroEvent
{
    internal CurrentPlayPositionEvent(TimeSpan inTimeSpan, int inTick)
    {
        EventType = GetType();
        timeSpan = inTimeSpan;
        tick = inTick;
    }

    public TimeSpan timeSpan { get; }

    public int tick { get; }

    public override bool IsValid()
    {
        return true;
    }
}