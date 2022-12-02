#region

using System.Collections.Generic;
using BardMusicPlayer.Transmogrify.Song.Config.Interfaces;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Config
{
    public class LyricProcessorConfig : IProcessorConfig
    {
        /// <inheritdoc />
        public int Track { get; set; } = 0;

        /// <inheritdoc />
        public List<long> IncludedTracks { get; set; } = new();

        /// <inheritdoc />
        public int PlayerCount { get; set; } = 1;
    }
}