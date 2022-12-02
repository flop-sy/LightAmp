#region

using Melanchall.DryWetMidi.Interaction;

#endregion

namespace BardMusicPlayer.Transmogrify.Processor.Utilities
{
    internal static partial class Extensions
    {
        /// <summary>
        /// </summary>
        /// <param name="note"></param>
        /// <param name="tempoMap"></param>
        /// <returns></returns>
        internal static long GetNoteMs(this TimedEvent note, TempoMap tempoMap)
        {
            return note.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;
        }
    }
}