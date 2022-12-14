namespace BardMusicPlayer.Maestro.Events;

public sealed class PerformersChangedEvent : MaestroEvent
{
    public bool Changed;

    internal PerformersChangedEvent()
    {
        EventType = GetType();
        Changed = true;
    }

    public override bool IsValid()
    {
        return true;
    }
}