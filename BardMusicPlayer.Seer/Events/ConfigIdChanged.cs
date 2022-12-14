#region

using System;

#endregion

namespace BardMusicPlayer.Seer.Events;

public sealed class ConfigIdChanged : SeerEvent
{
    internal ConfigIdChanged(EventSource readerBackendType, string configId) : base(readerBackendType)
    {
        EventType = GetType();
        ConfigId = configId;
    }

    public string ConfigId { get; }

    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(ConfigId) && ConfigId.StartsWith("FFXIV_CHR", StringComparison.Ordinal) &&
               ConfigId.Length == 25;
    }
}