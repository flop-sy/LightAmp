namespace BardMusicPlayer.Seer.Events;

public sealed class PlayerNameChanged : SeerEvent
{
    internal PlayerNameChanged(EventSource readerBackendType, string playerName) : base(readerBackendType)
    {
        EventType = GetType();
        PlayerName = playerName;
    }

    public string PlayerName { get; }

    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(PlayerName);
    }
}