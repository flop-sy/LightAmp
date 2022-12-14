#region

using System.Collections.Generic;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models.ReadResults;

internal sealed class ChatLogResult
{
    public List<ChatLogItem> ChatLogItems { get; } = new();

    public int PreviousArrayIndex { get; set; }

    public int PreviousOffset { get; set; }
}