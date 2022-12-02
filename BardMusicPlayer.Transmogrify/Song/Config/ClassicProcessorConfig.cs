#region

using System.Collections.Generic;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Song.Config.Interfaces;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Config
{
    public class ClassicProcessorConfig : IProcessorConfig
    {
        /// <summary>
        ///     The instrument for this track
        /// </summary>
        public Instrument Instrument { get; set; } = Instrument.Harp;

        /// <summary>
        ///     The octave range to use
        /// </summary>
        public OctaveRange OctaveRange { get; set; } = OctaveRange.C3toC6;

        /// <inheritdoc />
        public int Track { get; set; } = 0;

        /// <inheritdoc />
        public List<long> IncludedTracks { get; set; } = new();

        /// <inheritdoc />
        public int PlayerCount { get; set; } = 1;
    }
}