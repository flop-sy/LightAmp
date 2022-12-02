#region

using System.IO;
using System.Text;
using Melanchall.DryWetMidi.Core;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Utilities
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal static MidiFile ReadAsMidiFile(this Stream stream)
        {
            return MidiFile.Read(stream, new ReadingSettings
            {
                TextEncoding = Encoding.UTF8,
                InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore,
                InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.SnapToLimits,
                InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.SnapToLimits,
                InvalidSystemCommonEventParameterValuePolicy =
                    InvalidSystemCommonEventParameterValuePolicy.SnapToLimits,
                MissedEndOfTrackPolicy = MissedEndOfTrackPolicy.Ignore,
                NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore,
                UnexpectedTrackChunksCountPolicy = UnexpectedTrackChunksCountPolicy.Ignore,
                UnknownChannelEventPolicy = UnknownChannelEventPolicy.SkipStatusByteAndOneDataByte,
                UnknownChunkIdPolicy = UnknownChunkIdPolicy.ReadAsUnknownChunk
            });
        }
    }
}