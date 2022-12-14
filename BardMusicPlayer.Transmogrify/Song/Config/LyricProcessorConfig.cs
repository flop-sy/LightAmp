#region

using System.Collections.Generic;
using BardMusicPlayer.Transmogrify.Song.Config.Interfaces;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Config;

public sealed class LyricProcessorConfig : IProcessorConfig
{
    /// <inheritdoc />
    public int Track { get; set; }

    /// <inheritdoc />
    public List<long> IncludedTracks { get; set; } = new();

    /// <inheritdoc />
    public int PlayerCount { get; set; } = 1;
}