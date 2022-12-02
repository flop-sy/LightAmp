#region

using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;

#endregion

namespace BardMusicPlayer.Transmogrify.Song
{
    public sealed class TrackContainer
    {
        /// <summary>
        /// </summary>
        public TrackChunk SourceTrackChunk { get; set; } = new();

        /// <summary>
        /// </summary>
        public Dictionary<long, ConfigContainer> ConfigContainers { get; set; } = new();
    }
}