#region

using System.Collections.Generic;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Config.Interfaces
{
    public interface IProcessorConfig
    {
        /// <summary>
        ///     The main track this processorConfig is applied against
        /// </summary>
        int Track { get; set; }

        /// <summary>
        ///     Tracks to merge into the main track before processing
        /// </summary>
        List<long> IncludedTracks { get; set; }

        /// <summary>
        ///     Amount of players to distribute notes to.
        /// </summary>
        int PlayerCount { get; set; }
    }
}