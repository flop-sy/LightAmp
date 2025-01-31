namespace BardMusicPlayer.Seer.Events;

public sealed class MachinaManagerLogEvent : SeerEvent
{
    public MachinaManagerLogEvent(string message) : base(EventSource.MachinaManager)
    {
        EventType = GetType();
        Message = message;
    }

    public string Message { get; }

    public override bool IsValid()
    {
        return true;
    }
}